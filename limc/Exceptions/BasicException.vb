Public Class BasicException
    Inherits RenderableException

    Private Title As String

    Public Sub New(Title As String, Message As String)
        MyBase.New(Message)
        Me.Title = Title
    End Sub

    Public Overrides Sub Render()
        Console.ForegroundColor = ConsoleColor.DarkRed
        Console.WriteLine("[COMPILATION ERORR] " & Title)
        Console.ResetColor()
        Console.WriteLine(Message)
    End Sub

End Class
