Imports System.IO

Public Class LocalizedException
    Inherits CompilerException

    Private Location As Location

    Public Sub New(Name As String, Message As String, Location As Location)
        MyBase.New(Name, Message)
        Me.Location = Location
    End Sub

    Protected Overrides Sub PrintContent()

        'Print name & message
        MyBase.PrintContent()

        'File location or character location
        If TypeOf Location Is FileLocation Then

            'Print location
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine($"<{Path.GetRelativePath(".", Location.File.Filepath)}>")

        Else

            'Print trace
            If Location.ToLineNumber = Location.FromLineNumber Then
                PrintOneLine()
            Else
                PrintMultipleLine()
            End If

            'Print location
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine($"<{Path.GetRelativePath(".", Location.File.Filepath)}> at line {Location.FromLineNumber}")

        End If

    End Sub

    Private Sub PrintMultipleLine()

        'Set color
        Console.ForegroundColor = ConsoleColor.DarkGray

        'Read the line
        Dim reader As New StreamReader(Location.File.Filepath)

        'Read at line
        Dim i As Integer = 1
        While i < Location.FromLineNumber
            reader.ReadLine()
            i += 1
        End While
        For i = Location.FromLineNumber To Location.ToLineNumber
            Console.WriteLine($"{i} | {reader.ReadLine()}")
        Next

        'Close reader
        reader.Close()

    End Sub

    Private Sub PrintOneLine()

        'Read the line
        Dim reader As New StreamReader(Location.File.Filepath)

        'Reach line
        Dim i As Integer = 1
        While i < Location.FromLineNumber
            reader.ReadLine()
            i += 1
        End While

        'Length
        Dim Length = Location.ToCharIndex - Location.FromCharIndex
        If Length <= 0 Then
            Length = 1
        End If

        'Write line
        Dim Line As String = reader.ReadLine()
        Dim RemoveTabulation As Integer = 0
        While Line.Length > 0 AndAlso Char.IsWhiteSpace(Line(0))
            RemoveTabulation += 1
            Line = Line.Substring(1)
        End While
        Dim From As Integer = Location.FromCharIndex
        If From < RemoveTabulation Then
            From = RemoveTabulation
        End If

        Console.ForegroundColor = ConsoleColor.DarkGray
        Console.WriteLine($"{i.ToString("D4")} | {Line}")
        Console.ResetColor()
        Console.WriteLine($"       {StrDup(From - RemoveTabulation, " ")}{StrDup(Length, "^")}")

        'Close reader
        reader.Close()

    End Sub

End Class
