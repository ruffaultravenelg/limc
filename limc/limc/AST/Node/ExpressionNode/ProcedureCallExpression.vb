Namespace AST
    Friend Class ProcedureCallExpression
        Inherits ExpressionNode

        Public ReadOnly Property Procedure As ExpressionNode
        Public ReadOnly Property PassedArguments As IReadOnlyList(Of ExpressionNode)

        Public Sub New(Location As Location, Procedure As ExpressionNode, PassedArguments As IEnumerable(Of ExpressionNode))
            MyBase.New(Location)
            Me.Procedure = Procedure
            Me.PassedArguments = PassedArguments.ToList()
        End Sub

        Public Overloads Overrides Function Accept(Of D, R)(Visitor As IExpressionVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitProcedureCallExpression(Me, Data)
        End Function

    End Class
End Namespace