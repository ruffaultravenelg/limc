
Public Class UnexpectedTokenException
    Inherits SyntaxException

    Friend Sub New(Code As String, Message As String, Location As Location)
        MyBase.New(Code, Message, Location)
    End Sub
    Friend Sub New(Code As String, Token As Lexer.Token)
        MyBase.New(Code, $"Unexpected {Token.ToEnglish()}.", Token.Location)
    End Sub
    Friend Sub New(Code As String, GivenToken As Lexer.Token, Expected As Lexer.TokenType)
        MyBase.New(Code, $"A {Lexer.TokenTypeToEnglish(Expected)} was expected here, but a {GivenToken.ToEnglish()} was provided.", GivenToken.Location)
    End Sub
    Friend Sub New(Code As String, GivenToken As Lexer.Token, Expected As String)
        MyBase.New(Code, $"A {Expected.ToLower()} was expected here, but a {GivenToken.ToEnglish()} was provided.", GivenToken.Location)
    End Sub

End Class
