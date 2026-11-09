Namespace AST
    Friend Class MemberExpression
        Inherits ExpressionNode

        Public ReadOnly Property Element As ExpressionNode
        Public ReadOnly Property MemberName As String
        Public ReadOnly Property PassedGenerics As IReadOnlyList(Of TypeNode)

        Public Sub New(Element As ExpressionNode, MemberName As String, PassedGenerics As IEnumerable(Of TypeNode), Location As Location)
            MyBase.New(Location)
            Me.Element = Element
            Me.MemberName = MemberName
            Me.PassedGenerics = PassedGenerics.ToList()
        End Sub

        Public Overrides Function Accept(Of D, R)(Visitor As IExpressionVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitMemberExpression(Me, Data)
        End Function

    End Class
End Namespace