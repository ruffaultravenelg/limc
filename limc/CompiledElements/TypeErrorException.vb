Public Class TypeErrorException
    Inherits LocalizedException

    Public Sub New(Message As String, Location As Location)
        MyBase.New("Type error", Message, Location)
    End Sub

End Class
