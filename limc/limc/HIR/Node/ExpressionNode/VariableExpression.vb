Imports limc.Symbol

Namespace HIR
    Friend Class VariableExpression
        Inherits ExpressionNode

        Public ReadOnly Property VarSymbol As VariableSymbol

        Public Sub New(Location As Location, VarSymbol As VariableSymbol)
            MyBase.New(Location, VarSymbol.VariableType)
            Me.VarSymbol = VarSymbol
        End Sub

        Public Overrides Sub Accept(Visitor As IHIRVisitor)
            Visitor.VisitVariableExpression(Me)
        End Sub

    End Class
End Namespace