Namespace AST
    Friend Class VariableAssignStatement
        Inherits StatementNode

        Public ReadOnly Property Variable As ElementExpression
        Public ReadOnly Property NewValue As ExpressionNode

        Public Sub New(Variable As ExpressionNode, NewValue As ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.Variable = Variable
            Me.NewValue = NewValue
        End Sub

        Public Overloads Overrides Function Accept(Of D, R)(Visitor As IStatementVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitVariableAssignStatement(Me, Data)
        End Function

    End Class
End Namespace