Imports limc.Symbol

Namespace HIR
    Friend Class IntNode
        Inherits ExpressionNode

        Public ReadOnly Property Value As Integer

        Public Sub New(Location As Location, Value As Integer)
            MyBase.New(Location, TypeSymbol.INT)
            Me.Value = Value
        End Sub

        Public Overrides Sub Accept(Visitor As IHIRVisitor)
            Visitor.VisitIntNode(Me)
        End Sub

    End Class
End Namespace