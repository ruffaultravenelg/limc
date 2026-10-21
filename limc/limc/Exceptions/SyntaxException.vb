Public Class SyntaxException
    Inherits LocatedException

    Friend Sub New(Code As String, Message As String, Location As Location)
        MyBase.New(Code, "SYNTAX ERROR", Message, Location)
    End Sub

End Class
