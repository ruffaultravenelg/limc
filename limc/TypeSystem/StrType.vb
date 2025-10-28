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

    End Class
End Namespace