Namespace AST
    '
    ' Represent a list of statements (ex: body of a function)
    '
    Friend Class BlockStatement
        Inherits StatementNode

        Public ReadOnly Property Statements As IReadOnlyList(Of StatementNode)

        Public Sub New(Statements As IEnumerable(Of StatementNode))
            MyBase.New(New Location())
            Me.Statements = Statements
        End Sub

        Public Overloads Overrides Function Accept(Of D, R)(Visitor As IStatementVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitBlockStatement(Me, Data)
        End Function

    End Class
End Namespace