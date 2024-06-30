Public Class NotEnoughTokenException
    Inherits LocalizedException

    Public Sub New(Message As String, Location As Location)
        MyBase.New("It seems that the code is not complete", Message, Location)
    End Sub

End Class
