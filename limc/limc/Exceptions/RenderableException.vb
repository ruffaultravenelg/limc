Public MustInherit Class RenderableException
    Inherits Exception

    Public Sub New(Optional Message As String = "")
        MyBase.New(Message)
    End Sub

    Public MustOverride Sub Render()

End Class
