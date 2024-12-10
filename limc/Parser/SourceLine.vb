Public Class SourceLine

    ' Properties
    Public ReadOnly Tokens As IEnumerable(Of Token)
    Public ReadOnly Indentation As Integer
    Public ReadOnly Location As Location

    ' Constructor
    Public Sub New(Line As String, LineNumber As Integer, File As Lim.SourceFile)

        'Set location
        Me.Location = New Location(File, LineNumber)

        'Get indentation and remove it frome the line
        While Line.Length > 0 AndAlso Char.IsWhiteSpace(Line.First)
            Indentation += 1
            Line = Line.Substring(1)
        End While

        'Tokenize the line
        Me.Tokens = Tokenizer.TokenizeLine(Line, LineNumber, File)

    End Sub

    'Load a file
    Public Shared Function Load(File As Lim.SourceFile) As IEnumerable(Of SourceLine)

        'Read the file
        Dim Lines As String() = IO.File.ReadAllLines(File.FullFilePath)

        'Create the result
        Dim Result As New List(Of SourceLine)
        For Index = 0 To Lines.Count - 1
            Dim Line As New SourceLine(Lines(Index), Index, File)
            If Line.Tokens.Count > 0 Then
                Result.Add(Line)
            End If
        Next

        'Return result
        Return Result

    End Function

    'To string
    Public Overrides Function ToString() As String
        Return Location.ToString() & " | " & Indentation.ToString() & "> " & String.Join(" ", Tokens)
    End Function

End Class
