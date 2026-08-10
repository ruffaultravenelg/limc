Imports limc.AST

Namespace TypeSystem
    Public Class FunType
        Inherits Type

        Private Shared ExistingTypes As New List(Of FunType)

        Private ArgumentTypes As IEnumerable(Of Type)
        Public ReadOnly ReturnType As Type
        Private ReturnType_C As String

        Private FuncPtrType As String
        Private MethodPtrType As String
        Public Overrides ReadOnly Property IsPointer As Boolean = True

        Private Sub New(ArgumentTypes As IEnumerable(Of Type), ReturnType As Type)

            Me.ArgumentTypes = ArgumentTypes
            Me.ReturnType = ReturnType
            ReturnType_C = If(ReturnType Is Nothing, "void", ReturnType.cRepresentation)

            'Create C struct
            cStructName = CodeGen.Namer.Struct(ToString())
            Dim Args As String = ""
            For Each Arg In ArgumentTypes
                Args &= ", " & Arg.cRepresentation
            Next
            FuncPtrType = $"{ReturnType_C} (*func)({RUNTIME_CONTEXT_STRUCT_NAME}{Args})"
            MethodPtrType = $"{ReturnType_C} (*method)({RUNTIME_CONTEXT_STRUCT_NAME}, void*{Args})"
            Dim Fields As New List(Of String) From {
                $"union {{{FuncPtrType}; {MethodPtrType};}} u;",
                "void* instance;"
            }
            CodeGen.RegisterStruct(New CodeGen.Struct(cStructName, Fields, ToString()))

            'C representation
            Me.cRepresentation = $"{cStructName}*"

        End Sub
        Public Shared Function From(ArgumentTypes As IEnumerable(Of Type), ReturnType As Type) As FunType

            'Search if it already exist
            For Each T As FunType In ExistingTypes
                If T.ArgumentTypes.SequenceEqual(ArgumentTypes) AndAlso ReturnType = T.ReturnType Then
                    Return T
                End If
            Next

            'Create new type
            Dim Result As New FunType(ArgumentTypes, ReturnType)
            ExistingTypes.Add(Result)
            Return Result

        End Function

        Private ConstructorFromFunction As CodeGen.ContextedFunction = Nothing
        Public Function GetValueFromFunctionName(FunctionName As String) As String

            If ConstructorFromFunction Is Nothing Then
                ConstructorFromFunction = New CodeGen.ContextedFunction(
                    {FuncPtrType},
                    cRepresentation,
                    {
                        $"{cRepresentation} temp = LIM_ALLOC(sizeof({cStructName}));",
                        "temp->u.func = func;",
                        "temp->instance = NULL;",
                        "return temp;"
                    },
                    $"new {ToString()} (from function)"
                )
                CodeGen.RegisterFunction(ConstructorFromFunction)
            End If

            Return ConstructorFromFunction.WriteCall({FunctionName})

        End Function




        Private ConstructorsFromMethod As New Dictionary(Of String, CodeGen.ContextedFunction)
        Public Function GetValueFromMethodNameAndInstance(Scope As Context.Scope, MethodName As String, InstanceReference As String, InstanceType As TypeSystem.Type) As String

            If Not ConstructorsFromMethod.ContainsKey(MethodName) Then

                Dim UnwrapFunctionArgs As New List(Of String) From {$"void* instance_ptr"}
                Dim Args As String = RUNTIME_CONTEXT_VARIABLE_NAME & ", instance"
                For I As Integer = 0 To ArgumentTypes.Count - 1
                    UnwrapFunctionArgs.Add(ArgumentTypes(I).cRepresentation & " arg" & I)
                    Args &= ", arg" & I
                Next

                Dim UnwrapFunction As New CodeGen.ContextedFunction(
                    UnwrapFunctionArgs,
                    ReturnType_C,
                    {
                        $"{InstanceType.cRepresentation} instance = *(({InstanceType.cRepresentation}*)instance_ptr);",
                        $"return {MethodName}({Args});"
                    },
                    $"unwrap and execute {InstanceType.ToString()}.{ToString()} to {MethodName}"
                )

                Dim Wrapper As New CodeGen.ContextedFunction( 'TODO: this shit don't work with reference cause it will do pointer of pointer, TGC doesn't handle it well. And it's also really ugly
                    {$"{InstanceType.cRepresentation} instance"},
                    cRepresentation,
                    {
                        $"{InstanceType}* instance_ptr = LIM_ALLOC(sizeof({InstanceType.cRepresentation}));",
                        $"*instance_ptr = instance;",
                        $"{cRepresentation} temp = LIM_ALLOC(sizeof({cStructName}));",
                        $"temp->u.method = {UnwrapFunction.CompiledName};",
                        "temp->instance = (void*)instance_ptr;",
                        "return temp;"
                    },
                    $"new {InstanceType.ToString()}.{ToString()} to {MethodName}"
                )

                ConstructorsFromMethod(MethodName) = Wrapper

                CodeGen.RegisterFunction(UnwrapFunction)
                CodeGen.RegisterFunction(Wrapper)

            End If

            Return ConstructorsFromMethod(MethodName).WriteCall({InstanceReference})

        End Function




        Private ExecuteFunction As CodeGen.ContextedFunction = Nothing
        Public Function ExecuteProcedure(Writer As CWriter, Scope As Context.Scope, ProcedureObject As String, Arguments As IEnumerable(Of ExpressionNode)) As String

            'Execute function generation
            If ExecuteFunction Is Nothing Then
                Dim ExecuteFunctionArguments As New List(Of String) From {$"{cRepresentation} procedure_object"}
                Dim Args As String = ""
                For I As Integer = 0 To ArgumentTypes.Count - 1
                    ExecuteFunctionArguments.Add(ArgumentTypes(I).cRepresentation & " arg" & I)
                    Args &= ", arg" & I
                Next
                ExecuteFunction = New CodeGen.ContextedFunction(
                    ExecuteFunctionArguments,
                    ReturnType_C,
                    {
                        "if (procedure_object->instance == NULL) {",
                        vbTab & $"return procedure_object->u.func({RUNTIME_CONTEXT_VARIABLE_NAME}{Args});",
                        "} else {",
                        vbTab & $"return procedure_object->u.method({RUNTIME_CONTEXT_VARIABLE_NAME}, procedure_object->instance{Args});",
                        "}"
                    },
                    $"execute {ToString()}"
                )
                CodeGen.RegisterFunction(ExecuteFunction)
            End If

            'Check for argumenst (converstion)
            If ArgumentTypes.Count <> Arguments.Count Then
                Throw New SyntaxError("Function argument count mismatch", Scope.Location)
            End If
            Dim CompiledArgs As New List(Of String) From {ProcedureObject}
            For I As Integer = 0 To ArgumentTypes.Count - 1

                Dim ActualArgType As TypeSystem.Type = Arguments(I).GetExpressionReturnType(Scope)
                If ActualArgType <> ArgumentTypes(I) Then
                    Throw New TypeMismatchError(ArgumentTypes(I), ActualArgType, Arguments(I).Location)
                End If
                CompiledArgs.Add(Arguments(I).CompileExpression(Writer, Scope))

            Next

            Return ExecuteFunction.WriteCall(CompiledArgs)

        End Function

        Public Overrides ReadOnly Property cRepresentation As String
        Public ReadOnly Property cStructName As String

        Public Overrides Function DefaultValue(Scope As Context.Scope) As String
            Return "NULL"
        End Function

        Public Overrides Function ToString() As String
            Dim ArgTypes As String = String.Join(", ", ArgumentTypes.Select(Function(T) T.ToString()))
            Return $"fun<{ArgTypes}><{If(ReturnType Is Nothing, "", ReturnType.ToString())}>"
        End Function

    End Class

End Namespace