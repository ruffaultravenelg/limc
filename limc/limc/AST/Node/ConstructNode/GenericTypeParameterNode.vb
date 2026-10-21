Namespace AST
    Friend Class GenericTypeParameterNode
        Inherits ConstructNode

        Public ReadOnly Property Typename As String

        Public Sub New(Typename As String, Location As Location)
            MyBase.New(Location)
            Me.Typename = Typename
        End Sub

        Public Overloads Overrides Function Accept(Of D, R)(Visitor As IConstructVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitGenericTypeParameterNode(Me, Data)
        End Function
    End Class

End Namespace