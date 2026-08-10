Public Class NotTheRightAmountOfArgumentsException
    Inherits SyntaxError

    Public Sub New(GivenTypes As Integer, WantedTypes As Integer, Location As Location)
        MyBase.New($"{GivenTypes} arguments were given where {WantedTypes} {If(WantedTypes < 2, "was", "were")} expected.", Location)
    End Sub

End Class
