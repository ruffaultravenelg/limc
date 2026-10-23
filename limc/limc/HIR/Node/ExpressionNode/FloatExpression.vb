Imports limc.Environment

Namespace HIR
    Friend Class FloatExpression
        Inherits ExpressionNode

        Public ReadOnly Property Value As Double

        Public Sub New(Location As Location, Value As Double)
            MyBase.New(Location, Primitives.FLOAT)
            Me.Value = Value
        End Sub

        Public Overrides Sub Accept(Visitor As IHIRVisitor)
            Visitor.VisitFloatExpression(Me)
        End Sub

    End Class
End Namespace