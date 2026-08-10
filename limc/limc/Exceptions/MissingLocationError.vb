Public MustInherit Class MissingLocationError
    Inherits Exception

    Public Sub New(Optional Message As String = "")
        MyBase.New(Message)
    End Sub

    Public MustOverride Function CreateError(Location As Location) As RenderableException

End Class
