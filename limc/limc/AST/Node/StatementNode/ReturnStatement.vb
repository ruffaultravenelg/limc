Namespace AST
    Friend Class ReturnStatement
        Inherits StatementNode

        Public ReadOnly Property Value As ExpressionNode

        Public Sub New(Value As ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.Value = Value
        End Sub

        Public Overloads Overrides Function Accept(Of D, R)(Visitor As IStatementVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitReturnStatement(Me, Data)
        End Function

    End Class
End Namespace