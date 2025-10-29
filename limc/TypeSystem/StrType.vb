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
                        "str",
                        {},
                        Type.Str,
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

    End Class
End Namespace