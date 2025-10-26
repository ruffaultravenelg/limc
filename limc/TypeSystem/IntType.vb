Namespace TypeSystem
    Public Class IntType
        Inherits Type

        Public Overrides ReadOnly Property cRepresentation As String = "int"

        Public Overrides Function DefaultValue(Scope As Context.Scope) As String
            Return "0"
        End Function

        Public Overrides Function ToString() As String
            Return "int"
        End Function

    End Class
End Namespace