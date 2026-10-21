Namespace HIR
    Friend Class BlockStatement
        Inherits StatementNode

        Public ReadOnly Property Statements As IReadOnlyList(Of StatementNode)

        Public Sub New(Location As Location, Statements As IEnumerable(Of StatementNode))
            MyBase.New(Location)
            Me.Statements = Statements.ToList()
        End Sub

        Public Overrides Sub Accept(Visitor As IHIRVisitor)
            Visitor.VisitBlockStatmement(Me)
        End Sub

    End Class
End Namespace