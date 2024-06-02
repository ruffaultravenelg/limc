Public Class UnexpectedCharacterException
    Inherits LocalizedException

    Public Sub New(Message As String, Location As Location)
        MyBase.New("Unexpected character", Message, Location)
    End Sub


End Class
