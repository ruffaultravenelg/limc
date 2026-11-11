Namespace HIR
    Friend Class IntExpression
        Inherits ExpressionNode

        Public ReadOnly Property Value As Integer

        Public Sub New(Location As Location, Value As Integer, IntTypeSymb As Symbol.IntTypeSymbol)
            MyBase.New(Location, IntTypeSymb)
            Me.Value = Value
        End Sub

        Public Overrides Function Accept(Of R)(Visitor As IHIRExpressionVisitor(Of R)) As R
            Return Visitor.VisitIntExpression(Me)
        End Function

    End Class
End Namespace