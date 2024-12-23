Public Class Location

    ' Information of the location
    Public ReadOnly File As Lim.SourceFile
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

        'Get line
        Dim Line As String = GetLine()

        'Display the line
        Console.ForegroundColor = ConsoleColor.Gray
        Console.WriteLine(Line)

        'Display ^^^^^
        Console.ForegroundColor = ConsoleColor.Yellow
        Console.WriteLine(StrDup(Line.Length, "^"))

        'Reset color
        Console.ResetColor()

    End Sub

    'Combine
    Public Shared Operator +(a As Location, b As Location) As Location
        If TypeOf a Is PreciseLocation AndAlso TypeOf b Is PreciseLocation Then
            Return CType(a, PreciseLocation) + CType(b, PreciseLocation)
        End If
        Throw New NotImplementedException
    End Operator

    ' To string
    Public Overrides Function ToString() As String
        Return $"<{File.RelativePath}> at line {Line + 1}"
    End Function

End Class
