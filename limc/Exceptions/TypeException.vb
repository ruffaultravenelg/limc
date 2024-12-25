Public Class TypeException
    Inherits LocalizedException

    Public Sub New(Msg As String, Location As Location)
        MyBase.New("Type error", Msg, Location)
    End Sub

End Class
