Public Class ParsingLineUnexpectedElementException
    Inherits ParsingLineException

    Public Sub New(Message As String, LinePosition As Integer, Line As String, File As String)
        MyBase.New("Unexpected element while parsing a line", Message, LinePosition, Line, File)
    End Sub

End Class
