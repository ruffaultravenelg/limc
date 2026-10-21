Namespace AST
    Friend Class BooleanExpression
        Inherits ExpressionNode

        Public ReadOnly Property Value As Boolean

        Public Sub New(Value As Boolean, Location As Location)
            MyBase.New(Location)
            Me.Value = Value
        End Sub

        Public Overloads Overrides Function Accept(Of D, R)(Visitor As IExpressionVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitBooleanExpression(Me, Data)
        End Function

    End Class
End Namespace