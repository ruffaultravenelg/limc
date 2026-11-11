Imports limc.Symbol

Namespace HIR
    Friend Class VariableExpression
        Inherits ExpressionNode

        Public ReadOnly Property VarSymbol As VariableSymbol

        Public Sub New(Location As Location, VarSymbol As VariableSymbol)
            MyBase.New(Location, VarSymbol.VariableType)
            Me.VarSymbol = VarSymbol
        End Sub

        Public Overrides Function Accept(Of R)(Visitor As IHIRExpressionVisitor(Of R)) As R
            Return Visitor.VisitVariableExpression(Me)
        End Function

    End Class
End Namespace