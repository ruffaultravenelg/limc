Public Class LocalizedException
    Inherits DisplayableException

    Private Name As String
    Private Msg As String
    Private Location As Location
    Public Sub New(Name As String, Msg As String, Location As Location)
        Me.Name = Name
        Me.Msg = Msg
        Me.Location = Location
    End Sub

    Public Overrides Sub Display()
        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine("ERROR: " & Name)
        Console.ResetColor()
        Console.WriteLine(Msg)
        Console.WriteLine()
        Location.DisplayCodeSnippet()
        Console.WriteLine()
        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine(Location.ToString())
        Console.ResetColor()
    End Sub
End Class
