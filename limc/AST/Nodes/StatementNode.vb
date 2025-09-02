Namespace AST
    Public MustInherit Class StatementNode
        Inherits Node

        Public Sub New(Location As Location)
            MyBase.New(Location)
        End Sub

        Public MustOverride Sub Compile(Scope As Scope)

    End Class
End Namespace