Namespace TypeSystem
    Public MustInherit Class CommonType
        Inherits Type

        Public MustOverride Property GenericTypes As IEnumerable(Of TypeSystem.Type)

    End Class
End Namespace