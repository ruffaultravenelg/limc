Public Class UnexpectedTokenError
    Inherits LocatedError

    Public Sub New(Location As Location, Message As String)
        MyBase.New("unexpected token", Message, Location)
    End Sub
    Public Sub New(Location As Location, WantedTokenType As TokenType)
        Me.New(Location, GenerateMessageFromTokenType(WantedTokenType))
    End Sub

    Private Shared Function GenerateMessageFromTokenType(TokenType As TokenType) As String
        Select Case TokenType
            Case TokenType.TEXT
                Return "A name was expected here"
            Case TokenType.VAL_FLOAT
                Return "A floating point number was expected here"
            Case TokenType.VAL_INT
                Return "A number was expected here"
            Case TokenType.SYMBOL_LEFT_PARENTHESIS
                Return "An opening parenthesis was expected here"
            Case TokenType.SYMBOL_RIGHT_PARENTHESIS
                Return "A closing parenthesis was expected here"
            Case TokenType.SYMBOL_LESSTHAN
                Return "A '<' symbol was expected here"
            Case TokenType.SYMBOL_GREATERTHAN
                Return "A '>' symbol was expected here"
            Case TokenType.SYMBOL_COLON
                Return "A ':' symbol was expected here"
            Case TokenType.SYMBOL_COMMA
                Return "A ',' symbol was expected here"
            Case TokenType.SYMBOL_POINT
                Return "A '.' symbol was expected here"
            Case TokenType.SYMBOL_LEFT_BRACKETS
                Return "An opening bracket was expected here"
            Case TokenType.SYMBOL_RIGHT_BRACKETS
                Return "A closing bracket was expected here"
            Case TokenType.LINESTART
                Return "A new line was expected here"
            Case Else
                Return "Another element was expected here."
        End Select
    End Function

End Class
