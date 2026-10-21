
Public Class WrongIndentationExcepetion
    Inherits SyntaxException

    Friend Sub New(Code As String, GivenLevel As Integer, ExpectedLevel As Integer, Location As Location)
        MyBase.New(Code, $"The expected indentation level was a maximum of {ExpectedLevel}, but the line has an indentation of {GivenLevel}.", Location)
    End Sub

End Class
