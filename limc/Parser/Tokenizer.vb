Public Module Tokenizer

    'Properties of current tokenizer
    Private Line As String
    Private Index As Integer
    Private LineNumber As Integer
    Private File As Lim.SourceFile

    'Tokenize a line
    Public Function TokenizeLine(Line As String, LineNumber As Integer, File As Lim.SourceFile) As List(Of Token)

        'If the line contains a comment '//' -> remove it
        If Line.Contains("//") Then
            Line = Line.Substring(0, Line.IndexOf("//"))
        End If

        'If the line is empty, return an empty list
        If Line.Length = 0 Then
            Return New List(Of Token)
        End If

        'Set values
        Tokenizer.Line = Line
        Tokenizer.Index = 0
        Tokenizer.LineNumber = LineNumber
        Tokenizer.File = File

        'Parse all tokens
        Dim Result As New List(Of Token)
        While Index < Line.Length
            Result.Add(ParseToken())
        End While

        'Return result
        Return Result

    End Function

    'Current Char
    Private ReadOnly Property CurrentChar As Char
        Get
            Return If(Index < Line.Length, Line(Index), Nothing)
        End Get
    End Property

    'Advance character
    Private Sub Advance()
        Index += 1
    End Sub

    'Parse token
    Private Function ParseToken() As Token

        'Skip spaces
        While Char.IsWhiteSpace(CurrentChar)
            Advance()
        End While

        'If it's a number
        If Char.IsDigit(CurrentChar) Then
            Return ParseNumber()
        End If

        'If it's a string
        If CurrentChar = """"c Then
            Return ParseString()
        End If

        'If it's a word or a keyword
        If Char.IsLetter(CurrentChar) OrElse CurrentChar = "_"c Then
            Return ParseWord()
        End If



        'Don't know what it is
        Throw New SyntaxException($"Unexpected character '{CurrentChar}'.", New PreciseLocation(File, LineNumber, Index, 1))

    End Function

    'Parse word
    Private Function ParseWord() As Token

        'Get the word
        Dim StartIndex As Integer = Index
        Dim Word As String = ""
        While Char.IsLetterOrDigit(CurrentChar) OrElse CurrentChar = "_"
            Word &= CurrentChar
            Advance()
        End While

        'Word could be a keyword
        Dim EnumValue As String = "KEYWORD_" & Word.ToUpper()
        If [Enum].IsDefined(GetType(Token.TokenType), EnumValue) Then
            Return New Token([Enum].Parse(GetType(Token.TokenType), EnumValue), Word, New PreciseLocation(File, LineNumber, StartIndex, Index - StartIndex))
        End If

        'Maybe a boolean
        If Word.ToLower() = "true" Then
            Return New Token(Token.TokenType.VALUE_TRUE, New PreciseLocation(File, LineNumber, StartIndex, Index - StartIndex))
        ElseIf Word.ToLower() = "false" Then
            Return New Token(Token.TokenType.VALUE_FALSE, New PreciseLocation(File, LineNumber, StartIndex, Index - StartIndex))
        End If

        'Return the token
        Return New Token(Token.TokenType.WORD, Word, New PreciseLocation(File, LineNumber, StartIndex, Index - StartIndex))

    End Function

    'Parse string
    Private Function ParseString() As Token

        'Skip the first "
        Dim StartIndex As Integer = Index
        Advance()

        'Get the string
        Dim Str As String = ""
        While CurrentChar <> """"c And CurrentChar <> Nothing
            Str &= CurrentChar
            Advance()
        End While

        'Skip the last "
        Advance()

        'Return the token
        Return New Token(Token.TokenType.VALUE_STRING, Str, New PreciseLocation(File, LineNumber, StartIndex, Index - StartIndex))

    End Function

    'Parse number
    Private Function ParseNumber() As Token

        'Get the number
        Dim StartIndex As Integer = Index
        Dim Number As String = ""
        While Char.IsDigit(CurrentChar) OrElse CurrentChar = "."
            Number &= CurrentChar
            Advance()
        End While

        'Return the token
        If Number.Contains(".") Then
            Return New Token(Token.TokenType.VALUE_FLOAT, Number, New PreciseLocation(File, LineNumber, Index, Number.Length))
        Else
            Return New Token(Token.TokenType.VALUE_INT, Number, New PreciseLocation(File, LineNumber, Index, Number.Length))
        End If

    End Function

End Module
