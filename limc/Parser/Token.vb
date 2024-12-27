Public Class Token
    Implements ILocated

    ' Properties
    Public ReadOnly Property Type As TokenType
    Public ReadOnly Property Value As Object
    Public ReadOnly Property Location As Location Implements ILocated.Location

    ' Constructor
    Public Sub New(Type As TokenType, Location As Location)
        Me.Type = Type
        Me.Value = Nothing
        Me.Location = Location
    End Sub
    Public Sub New(Type As TokenType, Value As Object, Location As Location)
        Me.Type = Type
        Me.Value = Value
        Me.Location = Location
    End Sub

    'To string
    Public Overrides Function ToString() As String
        If Value IsNot Nothing Then
            Return "[" & Type.ToString() & ", " & Value.ToString() & "]"
        Else
            Return "[" & Type.ToString() & "]"
        End If
    End Function

    ' Token types
    Public Enum TokenType
        WORD
        LINESTART

        VALUE_INT
        VALUE_FLOAT
        VALUE_STRING
        VALUE_FSTRING
        VALUE_TRUE
        VALUE_FALSE

        OPERATOR_PLUS
        OPERATOR_MINUS
        OPERATOR_MULTIPLICATION
        OPERATOR_DIVISION
        OPERATOR_MODULO
        OPERATOR_IN
        OPERATOR_NOT
        OPERATOR_IS
        OPERATOR_EQUAL
        OPERATOR_LESSTHAN
        OPERATOR_LESSTHANEQUAL
        OPERATOR_MORETHAN
        OPERATOR_MORETHANEQUAL

        SYNTAX_LEFT_PARENTHESIS
        SYNTAX_RIGHT_PARENTHESIS
        SYNTAX_LEFT_BRACKET
        SYNTAX_RIGHT_BRACKET
        SYNTAX_DOT
        SYNTAX_COMMA
        SYNTAX_COLON
        SYNTAX_DOUBLECOLON
        SYNTAX_SOURCE

        KEYWORD_FUNC
        KEYWORD_CLASS
        KEYWORD_STRUCT
        KEYWORD_ENUM
        KEYWORD_IF
        KEYWORD_ELSE
        KEYWORD_ELSEIF
        KEYWORD_WHILE
        KEYWORD_FOR
        KEYWORD_FROM
        KEYWORD_TO
        KEYWORD_IMPORT
        KEYWORD_AS
        KEYWORD_RELATION
        KEYWORD_LET
        KEYWORD_GET
        KEYWORD_SET
        KEYWORD_RETURN
        KEYWORD_EXPORT
        KEYWORD_BREAK
        KEYWORD_CONTINUE
        KEYWORD_NEW

    End Enum

End Class
