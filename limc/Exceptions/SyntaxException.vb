Public Class SyntaxException
    Inherits LocalizedException

    Public Sub New(Msg As String, Location As Location)
        MyBase.New("Syntax error", Msg, Location)
    End Sub

End Class
