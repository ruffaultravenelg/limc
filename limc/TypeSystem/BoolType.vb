Namespace TypeSystem
    Public Class BoolType
        Inherits Type

        Public Overrides ReadOnly Property cRepresentation As String = "bool"

        Public Overrides Function DefaultValue(Scope As Context.Scope) As String
            Return "false"
        End Function

        Public Overrides Function ToString() As String
            Return "bool"
        End Function

        Protected Overrides ReadOnly Property Relations As IEnumerable(Of Lazy.Relation) = {}

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
                            $"return {Constants.INSTANCE_ARGUMENT_NAME} ? ""true"" : ""false"";"
                        }
                    )
                End If
                Return _str_method
            End Get
        End Property

    End Class
End Namespace