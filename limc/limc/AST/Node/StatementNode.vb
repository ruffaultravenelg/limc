Namespace AST
    Friend MustInherit Class StatementNode
        Inherits Node

        Public Sub New(Location As Location)
            MyBase.New(Location)
        End Sub

        Public MustOverride Overloads Function Accept(Of D, R)(Visitor As IStatementVisitor(Of D, R), Data As D) As R
        Public NotOverridable Overloads Overrides Function Accept(Of D, R)(Visitor As IASTVisitor(Of D, R), Data As D) As R
            Return Accept(DirectCast(Visitor, IStatementVisitor(Of D, R)), Data)
        End Function

    End Class
End Namespace