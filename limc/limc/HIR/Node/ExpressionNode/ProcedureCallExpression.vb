Imports limc.Symbol

Namespace HIR
    Friend Class ProcedureCallExpression
        Inherits ExpressionNode

        Public ReadOnly Property Procedure As ExpressionNode '.ExpressionReturnType is FuncTypeSymbol
        Public ReadOnly Property PassedArguments As IReadOnlyList(Of ExpressionNode)

        Public Sub New(Location As Location, Procedure As ExpressionNode, PassedArguments As IEnumerable(Of ExpressionNode))
            MyBase.New(Location, DirectCast(Procedure.ExpressionReturnType, FuncTypeSymbol).ProcReturnType)
            Me.Procedure = Procedure
            Me.PassedArguments = PassedArguments.ToList()
        End Sub

        Public Overrides Function Accept(Of R)(Visitor As IHIRExpressionVisitor(Of R)) As R
            Return Visitor.VisitProcedureCallExpression(Me)
        End Function

    End Class
End Namespace