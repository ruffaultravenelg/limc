Public Class UnexpectedEndOfFile
    Inherits InternalException

    Friend Sub New(Message As String)
        MyBase.New("LEXING ERROR", Message)
    End Sub

End Class
