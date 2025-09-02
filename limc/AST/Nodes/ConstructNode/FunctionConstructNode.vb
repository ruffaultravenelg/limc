Namespace AST
    Public Class FunctionConstructNode
        Inherits ConstructNode

        Private ReadOnly Property Name As String
        Private ReadOnly Property Arguments As IEnumerable(Of ArgumentNode)
        Private ReadOnly Property ReturnType As TypeNode
        Private ReadOnly Property Body As IEnumerable(Of StatementNode)

        Public Sub New(Name As String, Arguments As IEnumerable(Of ArgumentNode), ReturnType As TypeNode, Body As IEnumerable(Of StatementNode), Location As Location)
            MyBase.New(Location)
            Me.Name = Name
            Me.Arguments = Arguments
            Me.ReturnType = ReturnType
            Me.Body = Body
        End Sub

    End Class

End Namespace