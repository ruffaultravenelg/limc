Public Module TokenParser

    '
    ' Some constants
    '
    Private Const TOKENTYPE_KEYWORD_PREFIX As String = "KEYWORD_"
    Private Const TOKENTYPE_OPERATOR_PREFIX As String = "OPERATOR_"
    Private ReadOnly TokenTypesToRemoveNewLineAfter As TokenType() = {
        TokenType.PRECOMPILE,
        TokenType.SYNTAX_COMMA,
        TokenType.SYNTAX_LEFT_PARENTHESIS,
        TokenType.SYNTAX_LEFT_BRACKET,
        TokenType.SYNTAX_DOT
    }

    '
    ' Parse a list of lines into a list of tokens
    '
    Public Function Parse(ParsingFile As LimSource, Lines As IEnumerable(Of LimSourceLine)) As List(Of Token)

        'Create result
        Dim Result As New List(Of Token)

        'Set variables
        TokenParser.ParsingFile = ParsingFile
        Dim LastLineIndex As Integer = 0

        'Transform each line
        For Each Line As LimSourceLine In Lines

            'Prase tokens
            Dim Tokens As List(Of Token) = ParseLine(Line)

            'Add tokens
            If Tokens.Count > 0 Then
                Result.Add(New Token(New Location(ParsingFile, Line.LinePosition, 0, Line.LinePosition, Line.FirstContentCharPosition), TokenType.SYNTAX_LINESTART, Line.Tab)) 'Add a startline token with the indentation count
                Result.AddRange(Tokens)
            End If

            'Increase last line position (for last newline token)
            LastLineIndex += 1

        Next

        'Clean new lines
        RemoveSyntaxLineStartAfterSpecifiedTokens(Result)

        'Ensure the list ends with a SYNTAX_LINESTART token
        If Result.Count = 0 OrElse Result(Result.Count - 1).Type <> TokenType.SYNTAX_LINESTART OrElse Result(Result.Count - 1).Value <> 0 Then
            Result.Add(New Token(New Location(ParsingFile, LastLineIndex, 0, LastLineIndex, 0), TokenType.SYNTAX_LINESTART, 0))
        End If

        'Return result
        Return Result

    End Function

    '
    ' Remove new line after some tokens
    '
    Private Sub RemoveSyntaxLineStartAfterSpecifiedTokens(ByRef tokens As List(Of Token))

        'Set variables
        Dim i As Integer = 0

        'Loop through all tokens
        While i < tokens.Count - 1
            If TokenTypesToRemoveNewLineAfter.Contains(tokens(i).Type) AndAlso tokens(i + 1).Type = TokenType.SYNTAX_LINESTART Then
                tokens.RemoveAt(i + 1)
            Else
                i += 1
            End If
        End While

    End Sub

    '
    ' Variables for the parsing
    '
    Private CurrentChar As Char = Nothing
    Private CurrentLine As LimSourceLine
    Private CharIndex As Integer
    Private Tokens As List(Of Token)
    Private TokenCharIndexStart As Integer
    Private ParsingFile As LimSource

    '
    ' Parse a single line into a list of tokens
    '
    Private Function ParseLine(Line As LimSourceLine) As List(Of Token)

        'Set variables
        Tokens = New List(Of Token)
        CurrentLine = Line
        CharIndex = -1

        'Advance to the first character
        Advance()

        'Get token
        While Not CurrentChar = Nothing

            'Set the position of the start of the token
            TokenCharIndexStart = CharIndex

            ' Ignore whitespace
            If Char.IsWhiteSpace(CurrentChar) Then
                Advance()
                Continue While
            End If

            ' Handle words and keywords
            If Char.IsLetter(CurrentChar) OrElse CurrentChar = "_"c Then
                Tokens.Add(ParseWord())
                Continue While
            End If

            ' Handle numbers
            If Char.IsDigit(CurrentChar) OrElse (CurrentChar = "."c AndAlso Char.IsDigit(PeekNextChar())) Then
                Tokens.Add(ParseNumber())
                Continue While
            End If

            ' Handle strings
            If CurrentChar = """"c Then
                Tokens.Add(ParseString())
                Continue While
            End If

            ' Handle operators and punctuation
            Select Case CurrentChar

                Case "+"c
                    Tokens.Add(CreateToken(TokenType.OPERATOR_PLUS))
                Case "-"c
                    Tokens.Add(CreateToken(TokenType.OPERATOR_MINUS))
                Case "*"c
                    Tokens.Add(CreateToken(TokenType.OPERATOR_MULTIPLICATION))
                Case "/"c
                    Tokens.Add(CreateToken(TokenType.OPERATOR_DIVISION))
                Case "%"c
                    Tokens.Add(CreateToken(TokenType.OPERATOR_MODULO))

                Case "<"c
                    If PeekNextChar() = "="c Then
                        Advance()
                        Tokens.Add(CreateToken(TokenType.OPERATOR_LESSTHANEQUAL))
                    Else
                        Tokens.Add(CreateToken(TokenType.OPERATOR_LESSTHAN))
                    End If
                Case ">"c
                    If PeekNextChar() = "="c Then
                        Advance()
                        Tokens.Add(CreateToken(TokenType.OPERATOR_MORETHANEQUAL))
                    Else
                        Tokens.Add(CreateToken(TokenType.OPERATOR_MORETHAN))
                    End If

                Case "="c
                    Tokens.Add(CreateToken(TokenType.OPERATOR_EQUAL))

                Case "("c
                    Tokens.Add(CreateToken(TokenType.SYNTAX_LEFT_PARENTHESIS))
                Case ")"c
                    Tokens.Add(CreateToken(TokenType.SYNTAX_RIGHT_PARENTHESIS))
                Case "["c
                    Tokens.Add(CreateToken(TokenType.SYNTAX_LEFT_BRACKET))
                Case "]"c
                    Tokens.Add(CreateToken(TokenType.SYNTAX_RIGHT_BRACKET))
                Case "."c
                    Tokens.Add(CreateToken(TokenType.SYNTAX_DOT))
                Case ","c
                    Tokens.Add(CreateToken(TokenType.SYNTAX_COMMA))
                Case ":"c
                    Tokens.Add(CreateToken(TokenType.SYNTAX_COLON))

                Case "@"c
                    Tokens.Add(ParsePrecompile())

                Case Else
                    Throw New UnexpectedCharacterException("The '" & CurrentChar & "' character was unexpected in this context.", GetCurrentCharLocation())

            End Select

            ' Advance to the next character
            Advance()

        End While

        'Return the tokens
        Return Tokens

    End Function

    '
    ' Get current char location
    '
    Private Function GetCurrentCharLocation() As Location
        Return New Location(ParsingFile, CurrentLine.LinePosition, CurrentLine.FirstContentCharPosition + CharIndex, CurrentLine.LinePosition, CurrentLine.FirstContentCharPosition + CharIndex + 1)
    End Function

    '
    ' Create a new token
    '
    Private Function CreateToken(Type As TokenType, Optional Value As String = "") As Token
        Return New Token(New Location(ParsingFile, CurrentLine.LinePosition, CurrentLine.FirstContentCharPosition + TokenCharIndexStart, CurrentLine.LinePosition, CurrentLine.FirstContentCharPosition + CharIndex), Type, Value)
    End Function

    '
    ' Parse a word or keyword
    '
    Private Function ParseWord() As Token

        'Get the word
        Dim start As Integer = CharIndex
        While Char.IsLetterOrDigit(CurrentChar) OrElse CurrentChar = "_"c
            Advance()
        End While
        Dim word As String = CurrentLine.Content.Substring(start, CharIndex - start)

        ' Check if the word is a keyword or operator
        Dim tokenType As TokenType
        If [Enum].TryParse(TOKENTYPE_KEYWORD_PREFIX & word.ToUpper(), tokenType) Then
            Return CreateToken(tokenType)
        ElseIf [Enum].TryParse(TOKENTYPE_OPERATOR_PREFIX & word.ToUpper(), tokenType) Then
            Return CreateToken(tokenType)
        Else
            Return CreateToken(TokenType.WORD, word)
        End If

    End Function

    '
    ' Parse a precompile
    '
    Private Function ParsePrecompile() As Token

        'Pass the '@'
        Advance()

        'Get the word
        Dim start As Integer = CharIndex
        While Char.IsLetterOrDigit(CurrentChar)
            Advance()
        End While
        Dim word As String = CurrentLine.Content.Substring(start, CharIndex - start)

        'Create token
        Return CreateToken(TokenType.PRECOMPILE, word)

    End Function

    '
    ' Parse a number
    '
    Private Function ParseNumber() As Token

        'Create variables
        Dim start As Integer = CharIndex
        Dim isFloat As Boolean = False

        'Get actual number
        If CurrentChar = "."c Then
            isFloat = True
            Advance()
        End If
        While Char.IsDigit(CurrentChar) OrElse CurrentChar = "."c
            If CurrentChar = "."c Then
                If isFloat Then
                    Throw New UnexpectedCharacterException("Unexpected character '.' in number", GetCurrentCharLocation())
                End If
                isFloat = True
            End If
            Advance()
        End While

        'Get number string
        Dim number As String = CurrentLine.Content.Substring(start, CharIndex - start)

        'Create token
        If isFloat Then
            If number.StartsWith(".") Then
                Return CreateToken(TokenType.FLOAT, "0" & number)
            Else
                Return CreateToken(TokenType.FLOAT, number)
            End If
        Else
            Return CreateToken(TokenType.INT, number)
        End If

    End Function

    '
    ' Parse a string
    '
    Private Function ParseString() As Token

        ' Advance past the opening quote
        Advance()

        'Get the string
        Dim start As Integer = CharIndex
        While Not CurrentChar <> Nothing AndAlso CurrentChar <> """"c
            Advance()
        End While

        If CurrentChar = Nothing Then
            Throw New UnexpectedCharacterException("Unterminated string", GetCurrentCharLocation())
        End If

        Dim str As String = CurrentLine.Content.Substring(start, CharIndex - start)

        ' Advance past the closing quote
        Advance()

        'Create and return the token
        Return CreateToken(TokenType.STR, str)

    End Function

    '
    ' Peek the next character
    '
    Private Function PeekNextChar() As Char?
        If CharIndex + 1 < CurrentLine.Content.Length Then
            Return CurrentLine.Content(CharIndex + 1)
        Else
            Return Nothing
        End If
    End Function

    '
    ' Go to next character of the line
    '
    Private Sub Advance()
        CharIndex += 1
        If CharIndex < CurrentLine.Content.Length Then
            CurrentChar = CurrentLine.Content(CharIndex)
        Else
            CurrentChar = Nothing
        End If
    End Sub

End Module
