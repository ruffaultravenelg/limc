Public Class InvalidCompileTargetException
    Inherits CompilerException

    Public Sub New(Message As String)
        MyBase.New("Invalid compile target", Message)
    End Sub

End Class
