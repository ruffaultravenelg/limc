Public Class CWriter

    Private Lines As New List(Of String)

    Public Function GetLines() As IEnumerable(Of String)
        Return Lines
    End Function
    Public Function GetLinesIndented() As IEnumerable(Of String)
        Dim IndentedLines As New List(Of String)
        For Each line As String In Lines
            IndentedLines.Add("    " & line)
        Next
        Return IndentedLines
    End Function

    Public Sub WriteLine(Line As String)
        Lines.Add(Line)
    End Sub
    Public Sub WriteLines(Lines As IEnumerable(Of String))
        Me.Lines.AddRange(Lines)
    End Sub

    Public Sub AppendLastLine(Text As String)
        If Lines.Count = 0 Then
            Throw New InternalError()
        End If
        Lines(Lines.Count - 1) &= Text
    End Sub

End Class
