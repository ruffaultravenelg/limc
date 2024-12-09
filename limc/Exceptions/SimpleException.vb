Public Class SimpleException
    Inherits DisplayableException

    Private Name As String
    Private Msg As String
    Public Sub New(Name As String, Msg As String)
        Me.Name = Name
        Me.Msg = Msg
    End Sub

    Public Overrides Sub Display()
        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine("ERROR: " & Name)
        Console.ResetColor()
        Console.WriteLine(Msg)
    End Sub
End Class
