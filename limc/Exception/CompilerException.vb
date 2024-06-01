Public Class CompilerException
    Inherits Exception

    Private Name As String

    Public Sub New(Name As String, Message As String)
        MyBase.New(Message)
        Me.Name = Name
    End Sub

    Public Overridable Sub Print()

        'Print title
        Console.ForegroundColor = ConsoleColor.DarkRed
        Console.WriteLine("ERROR: " & Name)

        'Print message
        Console.ResetColor()
        Console.WriteLine(Message)

    End Sub


End Class
