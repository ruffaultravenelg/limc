Imports limc.Environment

Namespace HIR
    Friend Class BoolExpression
        Inherits ExpressionNode

        Public ReadOnly Property Value As Boolean

        Public Sub New(Location As Location, Value As Boolean)
            MyBase.New(Location, Primitives.BOOL)
            Me.Value = Value
        End Sub

        Public Overrides Sub Accept(Visitor As IHIRVisitor)
            Visitor.VisitBoolExpression(Me)
        End Sub

    End Class
End Namespace