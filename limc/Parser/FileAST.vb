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

    'Get a type of construct
    Public Iterator Function GetConstructs(Of T As ConstructNode)() As IEnumerable(Of T)
        For Each Obj As ConstructNode In Constructs
            If TypeOf Obj Is T Then
                Yield Obj
            End If
        Next
    End Function

    'Export all constructs if none of them are exported
    Public Sub HandleExports()

        'If at least one constuct is exported then exit the functions
        For Each Construct As ConstructNode In Constructs
            If Construct.Exported Then
                Return
            End If
        Next

        'Export all constructs
        For Each Construct As ConstructNode In Constructs
            Construct.SetExported(True)
        Next

    End Sub

End Class
