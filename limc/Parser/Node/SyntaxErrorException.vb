Public Class SyntaxErrorException
    Inherits LocalizedException

    Public Sub New(Message As String, Location As Location)
        MyBase.New("Syntax error", Message, Location)
    End Sub

End Class
