Namespace HIR
    Friend Class ReturnStatement
        Inherits StatementNode

        Public ReadOnly Property Value As ExpressionNode

        Public Sub New(Location As Location, Value As ExpressionNode)
            MyBase.New(Location)
            Me.Value = Value
        End Sub

        Public Overrides Sub Accept(Visitor As IHIRVisitor)
            Visitor.VisitReturnStatement(Me)
        End Sub

    End Class
End Namespace