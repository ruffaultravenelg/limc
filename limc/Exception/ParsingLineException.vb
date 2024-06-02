Imports System.IO

Public Class ParsingLineException
    Inherits CompilerException

    Private LinePosition As Integer
    Private Line As String
    Private File As String

    Public Sub New(Name As String, Message As String, LinePosition As Integer, Line As String, File As String)
        MyBase.New(Name, Message)
        Me.LinePosition = LinePosition
        Me.Line = Line
        Me.File = Path.GetRelativePath(".", File)
    End Sub

    Protected Overrides Sub PrintContent()

        'Print normal error
        MyBase.PrintContent()

        'Add print line
        Console.ForegroundColor = ConsoleColor.Gray
        Console.WriteLine($"{LinePosition} | {Line}")

        'Print trace
        Console.ForegroundColor = ConsoleColor.DarkRed
        Console.WriteLine($"<{File}> at line {LinePosition}")

    End Sub

End Class
