Namespace TypeSystem
    Public Class StrType
        Inherits Type

        Public Overrides ReadOnly Property cRepresentation As String = "char*"

        Public Overrides Function DefaultValue(Scope As Context.Scope) As String
            Return "'\0'"
        End Function

        Public Overrides Function ToString() As String
            Return "str"
        End Function

        Public Overrides Function RetrieveElements(Name As String) As IEnumerable(Of SearchMatch)
            Select Case Name
                Case "int"
                    Return {New SearchMatch(int_method)}
                Case Else
                    Return {}
            End Select
        End Function

        Private _int_method As Lazy.HardMethod = Nothing
        Private ReadOnly Property int_method As Lazy.HardMethod
            Get
                If _int_method Is Nothing Then
                    _int_method = New Lazy.HardMethod(
                        Me,
                        "int",
                        {},
                        Type.Int,
                        {
                            "char* endptr;",
                            "errno = 0;",
                            $"long val = strtol({Lazy.Method.METHOD_INSTANCE_ARGUMENT_NAME}, &endptr, 10);",
                            $"if (errno != 0 || *endptr != '\0' || val > INT_MAX || val < INT_MIN) {CodeGen.WritePanicCall("""Cannot convert str to long""")};",
                            "return (int)round(val);"
                        }
                    )
                End If
                Return _int_method
            End Get
        End Property

        Private _Relations As New List(Of Lazy.Relation)
        Protected Overrides ReadOnly Property Relations As IEnumerable(Of Lazy.Relation)
            Get
                If _Relations.Count = 0 Then

                    ' relation[index]
                    _Relations.Add(New Lazy.HardRelation(
                        Me,
                        RelationType.RELATION_BRACKETS,
                        {Type.Int},
                        {"index"},
                        Type.Str,
                        {
                            "size_t len = strlen(instance);",
                            "if (index < 0) index = len + index;",
                            $"if (index >= len || index < 0) {CodeGen.WritePanicCall("""Index out of range""")};",
                            $"char* result = {Constants.LIM_ALLOC}(2);",
                            "result[0] = instance[index];",
                            "result[1] = '\0';",
                            "return result;"
                        }
                    ))

                End If
                Return _Relations
            End Get
        End Property

    End Class
End Namespace