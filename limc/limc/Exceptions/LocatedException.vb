Public MustInherit Class LocatedException
    Inherits RenderableException

    Private Location As Location

    Friend Sub New(Code As String, Title As String, Message As String, Location As Location)
        MyBase.New(Code, Title, Message)
        Me.Location = Location
    End Sub

    Public Overrides Sub Render(Out As OutputStream.IOutputStream)

        ' Print title
        Out.SetColor(OutputStream.IOutputStream.Color.Red)
        Out.Write(Title.ToUpper())
        Out.Write(": ")
        Out.Writeline(Message)

        ' Print code preview
        Out.SetColor(OutputStream.IOutputStream.Color.White)
        For Each Line In Location.GetLines()
            Out.Writeline(Line)
        Next

        ' Print location
        Out.SetColor(OutputStream.IOutputStream.Color.Grey)
        Out.Writeline(Location.ToString())

    End Sub

End Class
