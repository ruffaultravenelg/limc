Namespace AST
    Friend Class FloatExpression
        Inherits ExpressionNode

        Public ReadOnly Property Value As Double

        Public Sub New(Value As Double, Location As Location)
            MyBase.New(Location)
            Me.Value = Value
        End Sub

        Public Overloads Overrides Function Accept(Of D, R)(Visitor As IExpressionVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitFloatExpression(Me, Data)
        End Function

    End Class
End Namespace