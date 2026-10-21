Imports System.IO

Namespace Lexer
    Friend Class Tokenizer

        Private Const DIGITS As String = "1234567890"
        Private Const DIGITS_WITH_POINT As String = DIGITS & "."
        Private Const IDENTIFIER_CHARS_PREFIX As String = "_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
        Private Const IDENTIFIER_CHARS As String = IDENTIFIER_CHARS_PREFIX & DIGITS

        ' ==== SHARED ====
        Public Shared Function Parse(Reader As TextReader, AbsoluteFilePath As String) As IEnumerable(Of Token)
            Dim Parser As New Tokenizer(Reader, AbsoluteFilePath)
            Dim Tokens = Parser.Process()
            Sanitizer.SanitizeTokenList(Tokens)
            Return Tokens
        End Function

        ' ==== CLASS ====

        Private Reader As CharReader
        Dim Result As List(Of Token)

        Private Sub New(Reader As TextReader, AbsoluteFilePath As String)
            Me.Reader = New CharReader(Reader, AbsoluteFilePath)
            Result = New List(Of Token)
        End Sub

        ' Get Location from current char
        Private Function GetCurrentCharLocation() As Location
            Return New Location(Reader.Filepath, Reader.LineIndex, Reader.ColIndex, 1)
        End Function

        ' Create token Helper functions
        Private Sub CreateToken(Type As TokenType, Value As Object, Length As Integer)
            Result.Add(New Token(Type, Value, New Location(Reader.Filepath, Reader.LineIndex, Reader.ColIndex, Length)))
        End Sub
        Private Sub CreateToken(Type As TokenType, Length As Integer)
            CreateToken(Type, "", Length)
        End Sub

        ' Text -> List(Of Token) # Do not sanitize -> maybe contains [NEWLINE] [NEWLINE]
        Public Function Process() As List(Of Token)
            CreateToken(TokenType.LINESTART, 0, 0)
            While True

                'Check for end of the file -> exit loop
                If Reader.EndOfFile Then
                    Exit While
                End If

                ' Comment
                If Reader.Current = "/"c AndAlso Reader.Peek = "/"c Then
                    While Reader.Current <> vbLf
                        Reader.Advance()
                    End While
                    Continue While
                End If

                ' Newline (reader normalize \cr & \crlf to \n)
                If Reader.Current = vbLf Then
                    Reader.Advance()

                    Dim Indentation As Integer = 0
                    While (Not Reader.EndOfFile) AndAlso (Reader.Current = " "c OrElse Reader.Current = vbTab)
                        Indentation += 1
                        Reader.Advance()
                    End While

                    CreateToken(TokenType.LINESTART, Indentation, Indentation)

                    If Reader.EndOfFile Then
                        Exit While
                    End If

                    Continue While

                End If

                ' Skip spaces
                If Char.IsWhiteSpace(Reader.Current) Then
                    Reader.Advance()
                    Continue While
                End If

                ' Get number
                If DIGITS.Contains(Reader.Current) Then
                    Dim Number As String = ""
                    Dim ContainsDot As Boolean = False

                    Do
                        If Reader.Current = "."c Then
                            If ContainsDot OrElse Not DIGITS.Contains(Reader.Peek) Then
                                Exit Do
                            Else
                                ContainsDot = True
                            End If
                        End If

                        Number &= Reader.Current
                        Reader.Advance()
                    Loop While DIGITS_WITH_POINT.Contains(Reader.Current)

                    Try
                        If ContainsDot Then
                            CreateToken(TokenType.FLOAT, Convert.ToDouble(Number), Number.Length)
                        Else
                            CreateToken(TokenType.INT, Convert.ToInt32(Number), Number.Length)
                        End If
                    Catch ex As FormatException
                        Throw New SyntaxException("L02", "Cannot convert term to a number", Result.Last.Location)
                    End Try

                    Continue While

                End If

                ' Raw string
                If Reader.Current = """"c Then
                    Reader.Advance()
                    Dim Res As String = ""
                    While Reader.Current <> """"c AndAlso Not Reader.EndOfFile
                        Res &= Reader.Current
                        Reader.Advance()
                    End While

                    If Reader.EndOfFile Then
                        Throw New UnexpectedEndOfFile("The string needs to be closed before the file end")
                    End If
                    Reader.Advance()

                    CreateToken(TokenType.RAW_STR, Res, Res.Length)
                    Continue While

                End If

                ' Formatted string
                If Reader.Current = "'"c Then
                    Reader.Advance()
                    Dim Res As String = ""
                    While Reader.Current <> "'"c AndAlso Not Reader.EndOfFile
                        Res &= Reader.Current
                        Reader.Advance()
                    End While

                    If Reader.EndOfFile Then
                        Throw New UnexpectedEndOfFile("The formatted string needs to be closed before the file end")
                    End If
                    Reader.Advance()

                    CreateToken(TokenType.FORMATTED_STR, Res, Res.Length)
                    Continue While

                End If

                ' Get keyword / identifier
                If IDENTIFIER_CHARS_PREFIX.Contains(Reader.Current) Then

                    Dim Word As String = ""
                    Do
                        Word &= Reader.Current
                        Reader.Advance()
                    Loop While IDENTIFIER_CHARS.Contains(Reader.Current)

                    Select Case Word.ToLower()
                        Case "func"
                            CreateToken(TokenType.KEYWORD_FUNC, Word.Length)
                        Case "let"
                            CreateToken(TokenType.KEYWORD_LET, Word.Length)
                        Case "import"
                            CreateToken(TokenType.KEYWORD_IMPORT, Word.Length)

                        Case Else
                            CreateToken(TokenType.IDENTIFIER, Word, Word.Length) 'Identifier (variable name, function name, etc.)
                    End Select

                    Continue While

                End If

                ' Generic tokens
                Select Case Reader.Current
                    Case "+"c
                        CreateToken(TokenType.PLUS, 1)
                    Case "-"c
                        CreateToken(TokenType.MINUS, 1)
                    Case "*"c
                        CreateToken(TokenType.MULTIPLICATION, 1)
                    Case "/"c
                        CreateToken(TokenType.DIVISION, 1)
                    Case "%"c
                        CreateToken(TokenType.MODULO, 1)
                    Case "("c
                        CreateToken(TokenType.OPENING_PARENTHESIS, 1)
                    Case ")"c
                        CreateToken(TokenType.CLOSING_PARENTHESIS, 1)
                    Case "["c
                        CreateToken(TokenType.OPENING_BRACKET, 1)
                    Case "]"c
                        CreateToken(TokenType.CLOSING_BRACKET, 1)
                    Case "{"c
                        CreateToken(TokenType.OPENING_BRACE, 1)
                    Case "}"c
                        CreateToken(TokenType.CLOSING_BRACE, 1)
                    Case ","c
                        CreateToken(TokenType.COMMA, 1)
                    Case "."c
                        CreateToken(TokenType.POINT, 1)
                    Case ":"c
                        CreateToken(TokenType.COLON, 1)
                    Case "="c
                        CreateToken(TokenType.EQUAL, 1)
                    Case "<"c
                        If Reader.Peek = "=" Then
                            Reader.Advance()
                            CreateToken(TokenType.LTE, 2)
                        Else
                            CreateToken(TokenType.LT, 1)
                        End If
                    Case ">"c
                        If Reader.Peek = "=" Then
                            Reader.Advance()
                            CreateToken(TokenType.GTE, 2)
                        Else
                            CreateToken(TokenType.GT, 1)
                        End If

                    Case Else
                        Throw New UnexpectedCharException(Reader.Current, GetCurrentCharLocation())

                End Select
                Reader.Advance()

                'Nothing after this cause the select doesn't contains "Continue While"

            End While

            CreateToken(TokenType.LINESTART, 0, 0)

            Return Result
        End Function

    End Class
End Namespace