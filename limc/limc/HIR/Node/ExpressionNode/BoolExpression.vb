Namespace HIR
    Friend Class BoolExpression
        Inherits ExpressionNode

        Public ReadOnly Property Value As Boolean

        Public Sub New(Location As Location, Value As Boolean, BoolSymbType As Symbol.BoolTypeSymbol)
            MyBase.New(Location, BoolSymbType)
            Me.Value = Value
        End Sub

        Public Overrides Sub Accept(Visitor As IHIRVisitor)
            Visitor.VisitBoolExpression(Me)
        End Sub

    End Class
End Namespace