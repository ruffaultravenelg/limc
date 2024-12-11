Public Class Location

    ' Information of the location
    Protected File As Lim.SourceFile
    Protected Line As Integer

    ' Constructor
    Public Sub New(File As Lim.SourceFile, Line As Integer)
        Me.File = File
        Me.Line = Line
    End Sub

    ' Returns the string of the line
    Protected Function GetLine() As String

        ' Read the file
        Dim Lines As String() = IO.File.ReadAllLines(File.FullFilePath)

        ' Return the line
        Return Lines(Line)

    End Function

    'Display the code snippet of the error
    Public Overridable Sub DisplayCodeSnippet()

        'Display the line
        Console.ForegroundColor = ConsoleColor.Gray
        Console.WriteLine(GetLine())

        'Reset color
        Console.ResetColor()

    End Sub

    ' To string
    Public Overrides Function ToString() As String
        Return $"<{File.RelativePath}> at line {Line + 1}"
    End Function

End Class
