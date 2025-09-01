Namespace AST
    Public Class FunctionConstructNode
        Inherits ConstructNode

        Private ReadOnly Property Name As String
        Private ReadOnly Property Arguments As IEnumerable(Of ArgumentNode)
        Private ReadOnly Property ReturnType As TypeNode
        Private ReadOnly Property Body As IEnumerable(Of StatementNode)

        Public Sub New(Location As Location)
            MyBase.New(Location)
        End Sub

    End Class

End Namespace