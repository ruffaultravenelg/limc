Imports System.Text

'
' Represent a lim file in the node form
'
Public Class FileAST

    'Constructs
    Private Constructs As New List(Of ConstructNode)

    'To string
    Public Overrides Function ToString() As String

        'Create string builder
        Dim Result As New StringBuilder()

        'Print each element to string
        For Each Obj In Constructs
            Result.Append(Obj.ToString())
            Result.Append(Environment.NewLine)
        Next

        'Return result
        Return Result.ToString()

    End Function

    'Append construct
    Public Sub AppendConstruct(Obj As ConstructNode)
        Constructs.Add(Obj)
    End Sub

    'Get a type of construct (alias)
    Public Function GetConstructs(Of T As ConstructNode)() As IEnumerable(Of T)
        Return ConstructNode.GetConstructsOfType(Of T)(Constructs)
    End Function

End Class
