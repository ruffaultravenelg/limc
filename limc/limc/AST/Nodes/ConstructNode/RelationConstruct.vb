Namespace AST
    Public Class RelationConstruct
        Inherits ConstructNode

        Public ReadOnly Property Type As TypeSystem.RelationType
        Public ReadOnly Property InstanceArgument As ArgumentNode
        Public ReadOnly Property Arguments As IEnumerable(Of ArgumentNode)
        Public ReadOnly Property ReturnType As TypeNode
        Public ReadOnly Property Body As IEnumerable(Of StatementNode)

        Public Sub New(Type As TypeSystem.RelationType, InstanceArgument As ArgumentNode, Arguments As IEnumerable(Of ArgumentNode), ReturnType As TypeNode, Body As IEnumerable(Of StatementNode), Location As Location)
            MyBase.New(Location)
            Me.Type = Type
            Me.InstanceArgument = InstanceArgument
            Me.Arguments = Arguments
            Me.ReturnType = ReturnType
            Me.Body = Body
        End Sub

        Public Overridable Function DoContainsStatement(Of T As StatementNode)()
            For Each Statement In Body
                If Statement.DoContainsStatement(Of T) Then
                    Return True
                End If
            Next
            Return False
        End Function

    End Class
End Namespace