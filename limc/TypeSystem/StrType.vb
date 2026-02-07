Namespace TypeSystem
    Public Class StrType
        Inherits Type

        Public Overrides ReadOnly Property cRepresentation As String = "char*"
        Public Overrides ReadOnly Property IsPointer As Boolean = True

        Public Overrides Function DefaultValue(Scope As Context.Scope) As String
            Return "NULL"
        End Function

        Public Overrides Function ToString() As String
            Return "str"
        End Function

        Public Sub Compile()
            RegisterMethod(New Lazy.HardMethod(Me, "int", {}, Type.Int, {
                "char* endptr;",
                "errno = 0;",
                $"long val = strtol({INSTANCE_ARGUMENT_NAME}, &endptr, 10);",
                $"if (errno != 0 || *endptr != '\0' || val > INT_MAX || val < INT_MIN) {CodeGen.WritePanicCall("""Cannot convert str to long""")};",
                "return (int)round(val);"
            }))
            RegisterGetter(New Lazy.DirectAccessGetter("len", Int, Function(instance) $"(int)strlen({instance})"))
        End Sub

        Private _Relations As New List(Of Lazy.Relation)
        Protected Overrides ReadOnly Property Relations As IEnumerable(Of Lazy.Relation)
            Get
                If _Relations.Count = 0 Then

                    ' instance[index]
                    _Relations.Add(New Lazy.HardRelation(
                        Me,
                        RelationType.RELATION_BRACKETS,
                        {Type.Int},
                        {"index"},
                        Type.Str,
                        {
                            $"size_t len = strlen({INSTANCE_ARGUMENT_NAME});",
                            "if (index < 0) index = len + index;",
                            $"if (index >= len || index < 0) {CodeGen.WritePanicCall("""Index out of range""")};",
                            $"char* result = {Constants.LIM_ALLOC}(2);",
                            $"result[0] = {INSTANCE_ARGUMENT_NAME}[index];",
                            "result[1] = '\0';",
                            "return result;"
                        }
                    ))

                    ' instance + string
                    _Relations.Add(New Lazy.HardRelation(
                        Me,
                        RelationType.RELATION_ADD,
                        {Type.Str},
                        {"other"},
                        Type.Str,
                        {
                            $"size_t len = strlen({INSTANCE_ARGUMENT_NAME}) + strlen(other);",
                            $"char* result = {Constants.LIM_ALLOC}(sizeof(char) * (len + 1));",
                            $"strcpy(result, {INSTANCE_ARGUMENT_NAME});",
                            "strcat(result, other);",
                            "return result;"
                        }
                    ))

                End If
                Return _Relations
            End Get
        End Property

    End Class
End Namespace