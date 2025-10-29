Imports limc.AST
Imports limc.Lazy

Namespace TypeSystem
    Public Class FunType
        Inherits Type

        Private Shared ExistingTypes As New List(Of FunType)

        Private ArgumentTypes As IEnumerable(Of Type)
        Public ReadOnly ReturnType As Type
        Private ReturnType_C As String

        Private FuncPtrType As String
        Private MethodPtrType As String

        Private Sub New(ArgumentTypes As IEnumerable(Of Type), ReturnType As Type)

            Me.ArgumentTypes = ArgumentTypes
            Me.ReturnType = ReturnType
            ReturnType_C = If(ReturnType Is Nothing, "void", ReturnType.cRepresentation)

            'Create C struct
            cStructName = CodeGen.Namer.Struct(ToString())
            Dim Args As String = RUNTIME_CONTEXT_STRUCT_NAME
            For Each Arg In ArgumentTypes
                Args &= ", " & Arg.cRepresentation
            Next
            FuncPtrType = $"{ReturnType_C} (*func)({Args})"
            MethodPtrType = $"{ReturnType_C} (*method)(void*, {Args})"
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

        Private ConstructorFromFunction As CodeGen.UtilFunction = Nothing
        Public Function GetValueFromFunctionName(FunctionName As String) As String

            If ConstructorFromFunction Is Nothing Then
                ConstructorFromFunction = New CodeGen.UtilFunction(
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

        Private ConstructorFromMethod As CodeGen.UtilFunction = Nothing
        Public Function GetValueFromMethodNameAndInstance(Scope As Context.Scope, MethodName As String, InstanceReference As String) As String

            If ConstructorFromMethod Is Nothing Then
                ConstructorFromMethod = New CodeGen.UtilFunction(
                    {MethodPtrType, "void* instance"},
                    cRepresentation,
                    {
                        $"{cRepresentation} temp = LIM_ALLOC(sizeof({cStructName}));",
                        "temp->u.method = method;",
                        "temp->instance = instance;",
                        "return temp;"
                    },
                    $"new {ToString()} (from instance)"
                )
                CodeGen.RegisterFunction(ConstructorFromMethod)
            End If

            Return ConstructorFromMethod.WriteCall({MethodName, $"(void*)({InstanceReference})"})

        End Function


        Private ExecuteFunction As CodeGen.UtilFunction = Nothing
        Public Function ExecuteProcedure(Scope As Context.Scope, ProcedureObject As String, Arguments As IEnumerable(Of ExpressionNode)) As String

            'Execute function generation
            If ExecuteFunction Is Nothing Then
                Dim ExecuteFunctionArguments As New List(Of String) From {$"{cRepresentation} procedure_object"}
                Dim Args As String = RUNTIME_CONTEXT_VARIABLE_NAME
                For I As Integer = 0 To ArgumentTypes.Count - 1
                    ExecuteFunctionArguments.Add(ArgumentTypes(I).cRepresentation & " arg" & I)
                    Args &= ", arg" & I
                Next
                ExecuteFunction = New CodeGen.UtilFunction(
                    ExecuteFunctionArguments,
                    ReturnType_C,
                    {
                        "if (procedure_object->instance == NULL) {",
                        vbTab & "return procedure_object->u.func(" & Args & ");",
                        "} else {",
                        vbTab & "return procedure_object->u.method(procedure_object->instance, " & Args & ");",
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
                CompiledArgs.Add(Arguments(I).CompileExpression(Scope))

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

        Public Overrides Function RetrieveElements(Name As String) As IEnumerable(Of SearchMatch)
            Return {}
        End Function

    End Class

End Namespace