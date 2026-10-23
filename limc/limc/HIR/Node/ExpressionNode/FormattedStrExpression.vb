Imports limc.Environment

Namespace HIR
    Friend Class FormattedStrExpression
        Inherits ExpressionNode

        Public ReadOnly Property Value As String

        Public Sub New(Location As Location, Value As String)
            MyBase.New(Location, Primitives.STR)
            Me.Value = Value
        End Sub

        Public Overrides Sub Accept(Visitor As IHIRVisitor)
            Visitor.VisitFormattedStrExpression(Me)
        End Sub

    End Class
End Namespace