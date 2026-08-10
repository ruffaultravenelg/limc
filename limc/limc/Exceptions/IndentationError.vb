Public Class IndentationError
    Inherits LocatedError

    Public Sub New(Location As Location, ExpectedIndentation As Integer)
        MyBase.New("Indentation missmatch", $"An indentation of {ExpectedIndentation}{If(ExpectedIndentation > 0, "or less", "")} was expected here.", Location)
    End Sub

End Class
