Imports limc.Environment

Namespace HIR
    Friend Class IntExpression
        Inherits ExpressionNode

        Public ReadOnly Property Value As Integer

        Public Sub New(Location As Location, Value As Integer)
            MyBase.New(Location, Primitives.INT)
            Me.Value = Value
        End Sub

        Public Overrides Sub Accept(Visitor As IHIRVisitor)
            Visitor.VisitIntExpression(Me)
        End Sub

    End Class
End Namespace