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

        Public Sub Compile()
            RegisterMethod(New Lazy.HardMethod(Me, "str", {}, Type.Str, {
                $"return {Constants.INSTANCE_ARGUMENT_NAME} ? ""true"" : ""false"";"
            }))
        End Sub

    End Class
End Namespace