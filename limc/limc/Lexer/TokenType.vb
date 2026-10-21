Namespace Lexer
    Friend Enum TokenType

        PLUS
        MINUS
        MULTIPLICATION
        DIVISION
        MODULO
        LT
        LTE
        GT
        GTE
        EQUAL
        OPENING_PARENTHESIS
        CLOSING_PARENTHESIS
        OPENING_BRACKET
        CLOSING_BRACKET
        OPENING_BRACE
        CLOSING_BRACE
        COMMA
        POINT
        COLON

        IDENTIFIER
        BOOL
        RAW_STR
        FORMATTED_STR
        INT
        FLOAT

        KEYWORD_FUNC
        KEYWORD_LET
        KEYWORD_IMPORT

        LINESTART

    End Enum

    Friend Module TokenTypeExtensions
        Public Function TokenTypeToEnglish(Type As TokenType) As String
            Select Case Type
                Case TokenType.PLUS
                    Return "plus symbol"
                Case TokenType.MINUS
                    Return "minus symbol"
                Case TokenType.MULTIPLICATION
                    Return "multiplication symbol"
                Case TokenType.DIVISION
                    Return "division symbol"
                Case TokenType.MODULO
                    Return "modulo symbol"
                Case TokenType.LT
                    Return "less-than symbol"
                Case TokenType.LTE
                    Return "less-than-equal symbol"
                Case TokenType.GT
                    Return "greater-than symbol"
                Case TokenType.GTE
                    Return "greater-then-equal symbol"
                Case TokenType.EQUAL
                    Return "equal symbol"
                Case TokenType.OPENING_PARENTHESIS
                    Return "opening parenthesis"
                Case TokenType.CLOSING_PARENTHESIS
                    Return "closing parenthesis"
                Case TokenType.OPENING_BRACKET
                    Return "opening bracket"
                Case TokenType.CLOSING_BRACKET
                    Return "closing bracket"
                Case TokenType.OPENING_BRACE
                    Return "opening curly brace"
                Case TokenType.CLOSING_BRACE
                    Return "closing curly brace"
                Case TokenType.COMMA
                    Return "comma"
                Case TokenType.POINT
                    Return "point"
                Case TokenType.COLON
                    Return "colon"
                Case TokenType.IDENTIFIER
                    Return "identifier"
                Case TokenType.BOOL
                    Return "boolean value"
                Case TokenType.RAW_STR
                    Return "string"
                Case TokenType.INT
                    Return "integer number"
                Case TokenType.FLOAT
                    Return "floating point number"
                Case TokenType.KEYWORD_FUNC
                    Return "func keyword"
                Case TokenType.LINESTART
                    Return "new line"
                Case Else
                    Return Type.ToString().ToLower()
            End Select
        End Function
    End Module

End Namespace