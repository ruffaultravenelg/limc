Namespace AST
    Public MustInherit Class StatementNode
        Inherits Node

        Public Sub New(Location As Location)
            MyBase.New(Location)
        End Sub

        Public MustOverride Sub Compile(Writer As CWriter, Scope As Context.Scope)

        Protected Overridable Function GetChildNodes() As IEnumerable(Of StatementNode)
            Return {}
        End Function
        Public Function DoContainsStatement(Of T As StatementNode)()
            If TypeOf Me Is T Then
                Return True
            End If
            For Each Statement In GetChildNodes()
                If Statement.DoContainsStatement(Of T) Then
                    Return True
                End If
            Next
            Return False
        End Function

    End Class
End Namespace