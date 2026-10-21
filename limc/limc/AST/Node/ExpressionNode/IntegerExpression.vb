Namespace AST
    Friend Class IntegerExpression
        Inherits ExpressionNode

        Public ReadOnly Property Value As Integer

        Public Sub New(Value As Integer, Location As Location)
            MyBase.New(Location)
            Me.Value = Value
        End Sub

        Public Overloads Overrides Function Accept(Of D, R)(Visitor As IExpressionVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitIntegerExpression(Me, Data)
        End Function

    End Class
End Namespace