Namespace AST
    Friend Class GenericElementExpression
        Inherits ExpressionNode

        Public ReadOnly Property ElementName As String
        Public ReadOnly Property PassedGenerics As IReadOnlyList(Of TypeNode)

        Public Sub New(ElementName As String, PassedGenerics As IEnumerable(Of TypeNode), Location As Location)
            MyBase.New(Location)
            Me.ElementName = ElementName
            Me.PassedGenerics = PassedGenerics.ToList()
        End Sub

        Public Overloads Overrides Function Accept(Of D, R)(Visitor As IExpressionVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitGenericElementExpression(Me, Data)
        End Function

    End Class
End Namespace