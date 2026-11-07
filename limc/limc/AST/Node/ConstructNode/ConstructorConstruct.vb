Namespace AST
    Friend Class ConstructorConstruct
        Inherits ConstructNode

        Public ReadOnly Property Arguments As IReadOnlyList(Of ArgumentNode)
        Public ReadOnly Property Body As BlockStatement

        Public Sub New(Arguments As IEnumerable(Of ArgumentNode), Body As BlockStatement, Location As Location)
            MyBase.New(Location)
            Me.Arguments = Arguments.ToList()
            Me.Body = Body
        End Sub

        Public Overloads Overrides Function Accept(Of D, R)(Visitor As IConstructVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitConstructorConstruct(Me, Data)
        End Function

    End Class

End Namespace