Public Class FileDoNotExistException
    Inherits CompilerException

    Public Sub New(Message As String)
        MyBase.New("File doesn't exist", Message)
    End Sub

End Class
