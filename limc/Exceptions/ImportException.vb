Public Class ImportException
    Inherits LocalizedException

    Public Sub New(Msg As String, Location As Location)
        MyBase.New("Import exception", Msg, Location)
    End Sub

End Class
