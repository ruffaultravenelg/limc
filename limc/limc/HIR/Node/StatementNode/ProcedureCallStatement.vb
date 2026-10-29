Namespace HIR
    Friend Class ProcedureCallStatement
        Inherits StatementNode

        Public ReadOnly Property Procedure As ExpressionNode '.ExpressionReturnType is FuncTypeSymbol
        Public ReadOnly Property PassedArguments As IReadOnlyList(Of ExpressionNode)

        Public Sub New(Location As Location, Procedure As ExpressionNode, PassedArguments As IEnumerable(Of ExpressionNode))
            MyBase.New(Location)
            Me.Procedure = Procedure
            Me.PassedArguments = PassedArguments.ToList()
        End Sub

        Public Overrides Sub Accept(Visitor As IHIRVisitor)
            Visitor.VisitProcedureCallStatement(Me)
        End Sub

    End Class
End Namespace