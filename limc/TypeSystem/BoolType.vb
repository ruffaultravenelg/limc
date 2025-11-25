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

        Public Overrides Function RetrieveElements(Name As String) As IEnumerable(Of SearchMatch)
            Return {}
        End Function

        Protected Overrides ReadOnly Property Relations As IEnumerable(Of Lazy.Relation) = {}


    End Class
End Namespace