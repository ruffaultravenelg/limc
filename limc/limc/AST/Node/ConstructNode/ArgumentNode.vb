Namespace AST
    Friend Class ArgumentNode
        Inherits ConstructNode

        Public ReadOnly Property ArgumentName As String
        Public ReadOnly Property ArgumentType As GenericTypeNode

        Public Sub New(ArgumentName As String, ArgumentType As GenericTypeNode, Location As Location)
            MyBase.New(Location)
            Me.ArgumentName = ArgumentName
            Me.ArgumentType = ArgumentType
        End Sub

        Public Overloads Overrides Function Accept(Of D, R)(Visitor As IConstructVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitArgumentNode(Me, Data)
        End Function
    End Class

End Namespace