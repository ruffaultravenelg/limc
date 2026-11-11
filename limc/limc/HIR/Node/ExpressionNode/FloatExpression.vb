Namespace HIR
    Friend Class FloatExpression
        Inherits ExpressionNode

        Public ReadOnly Property Value As Double

        Public Sub New(Location As Location, Value As Double, FloatTypeSymb As Symbol.FloatTypeSymbol)
            MyBase.New(Location, FloatTypeSymb)
            Me.Value = Value
        End Sub

        Public Overrides Function Accept(Of R)(Visitor As IHIRExpressionVisitor(Of R)) As R
            Return Visitor.VisitFloatExpression(Me)
        End Function

    End Class
End Namespace