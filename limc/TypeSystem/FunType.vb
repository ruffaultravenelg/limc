Imports limc.AST

Namespace TypeSystem
    Public Class FunType
        Inherits Type

        Private Shared ExistingTypes As New List(Of FunType)

        Private ArgumentTypes As IEnumerable(Of Type)
        Public ReadOnly ReturnType As Type

        Private FuncPtrType As String
        Private MethodPtrType As String

        Private Sub New(ArgumentTypes As IEnumerable(Of Type), ReturnType As Type)

            Me.ArgumentTypes = ArgumentTypes
            Me.ReturnType = ReturnType

            'Create C struct
            Dim StructName As String = CodeGen.Namer.Struct(ToString())
            Dim Args As String = String.Join(", ", ArgumentTypes.Select(Function(T) T.cRepresentation))
            FuncPtrType = $"{ReturnType.cRepresentation} (*func)({Args})"
            MethodPtrType = $"{ReturnType.cRepresentation} (*method)(void*, {Args})"
            Dim Fields As New List(Of String) From {
                $"union {{{FuncPtrType}; {MethodPtrType};}} u;",
                "void* instance;"
            }
            CodeGen.RegisterStruct(New CodeGen.Struct(StructName, Fields, ToString()))

            'C representation
            Me.cRepresentation = $"{StructName}*"

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

        Private FunctionConstructor As String = ""
        Public Function GetValueFromFunctionName(FunctionName As String) As String

            If FunctionConstructor = "" Then
                FunctionConstructor = $"new_{cRepresentation}_func"
                CodeGen.RegisterFunction(New CodeGen.Function(
                    $"{cRepresentation} {FunctionConstructor}({FuncPtrType})",
                    {
                        $"{cRepresentation} temp = LIM_ALLOC(sizeof({cRepresentation}));",
                        "temp->u.func = func;",
                        "temp->instance = NULL;",
                        "return temp;"
                    }
                ))
            End If

            Return $"{FunctionConstructor}({FunctionName})"

        End Function

        Private MethodConstructor As String = ""
        Public Function GetValueFromMethodNameAndInstance(Scope As Scope, MethodName As String, InstanceReference As String) As String

            If MethodConstructor = "" Then
                MethodConstructor = $"new_{cRepresentation}_method"
                CodeGen.RegisterFunction(New CodeGen.Function(
                    $"{cRepresentation} {MethodConstructor}({MethodPtrType}, void* instance)",
                    {
                        $"{cRepresentation} temp = LIM_ALLOC(sizeof({cRepresentation}));",
                        "temp->u.method = method;",
                        "temp->instance = instance;",
                        "return temp;"
                    }
                ))
            End If

            Return $"{MethodConstructor}({MethodName}, (void*)({InstanceReference}))"

        End Function


        Private ExecuteFunctionName As String = ""
        Public Function ExecuteFunction(Scope As Scope, ProcedureObject As String, Arguments As IEnumerable(Of ExpressionNode)) As String

            'Execute function generation
            If ExecuteFunctionName = "" Then
                ExecuteFunctionName = $"{cRepresentation}_execute"
                Dim ExecuteFunctionArguments As String = ""
                Dim Args As String = ""
                For I As Integer = 0 To ArgumentTypes.Count - 1
                    ExecuteFunctionArguments &= ", " & ArgumentTypes(I).cRepresentation & " arg" & I
                    Args &= ", arg" & I
                Next
                If Args.StartsWith(", ") Then
                    Args = Args.Substring(2)
                End If
                CodeGen.RegisterFunction(New CodeGen.Function(
                    $"{ReturnType.cRepresentation} {ExecuteFunctionName}({cRepresentation} procedure_object{ExecuteFunctionArguments})",
                    {
                        "if (procedure_object->instance == NULL)",
                        vbTab & "return procedure_object->u.func(" & Args & ");",
                        "else",
                        vbTab & "return procedure_object->u.method(procedure_object->instance, " & Args & ");"
                    }
                ))
            End If

            'Check for argumenst (converstion)
            If ArgumentTypes.Count <> Arguments.Count Then
                Throw New SyntaxError("Function argument count mismatch", Scope.Location)
            End If
            Dim CompiledArgs As String = ""
            For I As Integer = 0 To ArgumentTypes.Count - 1

                Dim ActualArgType As TypeSystem.Type = Arguments(I).GetExpressionReturnType(Scope)
                If ActualArgType <> ArgumentTypes(I) Then
                    Throw New TypeMismatchError(ArgumentTypes(I), ActualArgType, Arguments(I).Location)
                End If
                CompiledArgs &= ", " & Arguments(I).CompileExpression(Scope)

            Next
            If CompiledArgs.StartsWith(", ") Then
                CompiledArgs = CompiledArgs.Substring(2)
            End If

            Return $"{ExecuteFunctionName}({CompiledArgs})"

        End Function

        Public Overrides ReadOnly Property cRepresentation As String

        Public Overrides Function DefaultValue(Scope As Scope) As String
            Return "NULL"
        End Function

        Public Overrides Function ToString() As String
            Dim ArgTypes As String = String.Join(", ", ArgumentTypes.Select(Function(T) T.ToString()))
            Return $"fun<{ArgTypes}><{ReturnType}>"
        End Function

    End Class

End Namespace