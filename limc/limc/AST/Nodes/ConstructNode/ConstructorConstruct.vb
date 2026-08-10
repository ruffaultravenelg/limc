Namespace AST
    Public Class ConstructorConstruct
        Inherits ConstructNode

        Public ReadOnly Property Arguments As IEnumerable(Of ArgumentNode)
        Public ReadOnly Property Body As IEnumerable(Of StatementNode)

        Public Sub New(Arguments As IEnumerable(Of ArgumentNode), Body As IEnumerable(Of StatementNode), Location As Location)
            MyBase.New(Location)
            Me.Arguments = Arguments
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