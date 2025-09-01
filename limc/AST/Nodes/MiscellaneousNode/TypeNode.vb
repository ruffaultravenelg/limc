Namespace AST
    Public MustInherit Class TypeNode
        Inherits Node

        Public Sub New(Location As Location)
            MyBase.New(Location)
        End Sub

        Public MustOverride Function GetAssociatedType(Context As Context) As TypeSystem.Type

    End Class
End Namespace