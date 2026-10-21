Public Class ArgumentParsingException
    Inherits RenderableException

    Friend Sub New(Code As String, Message As String)
        MyBase.New(Code, "Argument parsing error", Message)
    End Sub

End Class
