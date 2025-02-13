Public Class ElementNotFoundException
    Inherits LocalizedException

    Public Sub New(Msg As String, Location As Location)
        MyBase.New("Element not found", Msg, Location)
    End Sub

End Class
