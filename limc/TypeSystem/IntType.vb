Namespace TypeSystem
    Public Class IntType
        Inherits Type

        Const INT_TO_STR_BUFFERSIZE As String = "12"

        Public Overrides ReadOnly Property cRepresentation As String = "int"

        Public Overrides Function DefaultValue(Scope As Context.Scope) As String
            Return "0"
        End Function

        Public Overrides Function ToString() As String
            Return "int"
        End Function

        Public Overrides Function RetrieveElements(Name As String) As IEnumerable(Of SearchMatch)
            Select Case Name
                Case "str"
                    Return {New SearchMatch(str_method)}
                Case Else
                    Return {}
            End Select
        End Function

        Private _str_method As Lazy.HardMethod = Nothing
        Private ReadOnly Property str_method As Lazy.HardMethod
            Get
                If _str_method Is Nothing Then
                    _str_method = New Lazy.HardMethod(
                       Me,
                        "str",
                        {},
                        Type.Str,
                        {
                            $"char* buffer = {CodeGen.LIM_ALLOC}({INT_TO_STR_BUFFERSIZE});",
                            $"sprintf(buffer, ""%d"", {Lazy.Method.METHOD_INSTANCE_ARGUMENT_NAME});",
                            "return buffer;"
                        }
                    )
                End If
                Return _str_method
            End Get
        End Property

    End Class
End Namespace