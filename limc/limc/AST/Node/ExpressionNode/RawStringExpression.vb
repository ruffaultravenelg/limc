Namespace AST
    Friend Class RawStringExpression
        Inherits ExpressionNode

        Public ReadOnly Property Value As String

        Public Sub New(Value As String, Location As Location)
            MyBase.New(Location)
            Me.Value = Value
        End Sub

        Public Overloads Overrides Function Accept(Of D, R)(Visitor As IExpressionVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitRawStringExpression(Me, Data)
        End Function

    End Class
End Namespace