Public Class PreciseLocation
    Inherits Location

    ' Information of the location
    Private Column As Integer
    Private Length As Integer

    ' Constructor
    Public Sub New(File As Lim.SourceFile, Line As Integer, Column As Integer, Length As Integer)
        MyBase.New(File, Line)
        Me.Column = Column
        Me.Length = Length
    End Sub

    'Display the code snippet of the error
    Public Overrides Sub DisplayCodeSnippet()

        'Display the line
        Console.ForegroundColor = ConsoleColor.Gray
        Console.WriteLine(GetLine())

        'Display ^^^^^
        Console.ForegroundColor = ConsoleColor.Yellow
        Console.WriteLine(StrDup(Column, " ") & StrDup(Length, "^"))

        'Reset color
        Console.ResetColor()

    End Sub

End Class
