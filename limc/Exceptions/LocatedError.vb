Imports System.IO

Public Class LocatedError
    Inherits RenderableException

    Private Title As String
    Private Location As Location

    Public Sub New(Title As String, Message As String, Location As Location)
        MyBase.New(Message)
        Me.Title = Title
        Me.Location = Location
    End Sub

    Public Overrides Sub Render()

        Console.ForegroundColor = ConsoleColor.DarkRed
        Console.WriteLine("[COMPILATION ERROR] " & Title)
        Console.ResetColor()
        Console.WriteLine(Message)

        If Location.ToLineNumber - Location.FromLineNumber = 0 Then
            RenderOneLine()
        Else
            RenderMultipleLines()
        End If

        Console.ForegroundColor = ConsoleColor.DarkRed
        Console.WriteLine($"<{Location.File.Filename}> line {Location.FromLineNumber + 1} character {Location.FromCol}")
        Console.ResetColor()

    End Sub

    Private Sub RenderOneLine()

        'Read line
        Dim line As String = ""
        Try
            Using Reader As New StreamReader(Location.File.Filepath)
                For i As Integer = 0 To Location.FromLineNumber - 1
                    Reader.ReadLine()
                Next
                line = Reader.ReadLine()
            End Using
        Catch ex As Exception
            Console.WriteLine("> unreatchable line")
        End Try

        'Replace tabs by space to make each character 1 long
        line = line.Replace(vbTab, " "c)

        'Print
        Console.WriteLine()
        Console.WriteLine(vbTab & $"{Location.FromLineNumber + 1} | ""{line}""")
        Console.ForegroundColor = ConsoleColor.White
        Console.WriteLine(vbTab & StrDup((Location.FromLineNumber + 1).ToString().Length + 4, " ") & StrDup(Location.FromCol, " "c) & StrDup(Location.ToCol - Location.FromCol, "^"c))
        Console.ResetColor()
        Console.WriteLine()

    End Sub

    Private Sub RenderMultipleLines()

        'Read & write lines
        Console.WriteLine()
        Dim line As String = ""
        Try
            Using Reader As New StreamReader(Location.File.Filepath)
                For i As Integer = 0 To Location.FromLineNumber - 1
                    Reader.ReadLine()
                Next
                For i As Integer = Location.FromLineNumber To Location.ToLineNumber
                    Console.WriteLine(vbTab & $"{i} | ""{Reader.ReadLine().Replace(vbTab, " "c)}""")
                Next
            End Using
        Catch ex As Exception
            Console.WriteLine("> unreatchable line")
        End Try
        Console.WriteLine()

    End Sub

End Class
