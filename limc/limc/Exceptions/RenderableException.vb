Public Class RenderableException
    Inherits InternalException

    Protected ReadOnly Code As String

    Friend Sub New(Code As String, Title As String, Message As String)
        MyBase.New(Title, Message)
        Me.Code = Code
    End Sub

    Public Overridable Sub Render(Out As OutputStream.IOutputStream)

        ' Print title
        Out.SetColor(OutputStream.IOutputStream.Color.Red)
        Out.Write(Title.ToUpper())
        Out.Write(": ")
        Out.ResetColor()
        Out.Writeline(Message)

    End Sub

End Class
