Namespace AST
    '
    ' Represent a type reference
    ' variable:list<str>
    '          ^^^^^^^^^
    Friend Class GenericTypeNode
        Inherits TypeNode

        Public ReadOnly Property Name As String
        Public ReadOnly Property Arguments As IEnumerable(Of GenericTypeNode)

        Public Sub New(Name As String, Arguments As IEnumerable(Of GenericTypeNode), Location As Location)
            MyBase.New(Location)
            Me.Name = Name
            Me.Arguments = Arguments
        End Sub

        Public Overloads Overrides Function Accept(Of D, R)(Visitor As ITypeVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitGenericTypeNode(Me, Data)
        End Function

    End Class
End Namespace