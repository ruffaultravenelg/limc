Namespace AST
    Public Class FunctionConstruct
        Inherits ConstructNode

        Public ReadOnly Property Name As String
        Public ReadOnly Property GenericArguments As IEnumerable(Of String)
        Public ReadOnly Property Arguments As IEnumerable(Of ArgumentNode)
        Public ReadOnly Property ReturnType As TypeNode
        Public ReadOnly Property Body As IEnumerable(Of StatementNode)

        Public Sub New(Name As String, GenericArguments As IEnumerable(Of String), Arguments As IEnumerable(Of ArgumentNode), ReturnType As TypeNode, Body As IEnumerable(Of StatementNode), Location As Location)
            MyBase.New(Location)
            Me.Name = Name
            Me.GenericArguments = GenericArguments
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