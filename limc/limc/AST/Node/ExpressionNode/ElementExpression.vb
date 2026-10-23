Namespace AST
    Friend Class ElementExpression
        Inherits ExpressionNode

        Public ReadOnly Property ElementName As String

        Public Sub New(ElementName As String, Location As Location)
            MyBase.New(Location)
            Me.ElementName = ElementName
        End Sub

        Public Overloads Overrides Function Accept(Of D, R)(Visitor As IExpressionVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitElementExpression(Me, Data)
        End Function

    End Class
End Namespace