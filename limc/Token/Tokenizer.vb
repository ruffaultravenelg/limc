Imports System.IO
Imports System.Text
Imports limc.Context

Public Module Tokenizer

    Private Const DIGITS As String = "1234567890"
    Private Const DIGITS_WITH_POINT As String = DIGITS & "."
    Private Const ALPHABET As String = "azertyuiopmlkjhgfdsqwxcvbn"
    Private ReadOnly VALID_TEXT_CHARS As String = ALPHABET & ALPHABET.ToUpper() & "_"
    Private ReadOnly VALID_TEXT_CHARS_AFTER_FIRST_LETTER As String = VALID_TEXT_CHARS & DIGITS

    Private Source As SourceFile
    Private Results As List(Of Token)
    Private LineNumber As Integer = 0

    Public Function TokenizeFile(Source As SourceFile) As IEnumerable(Of Token)
        Tokenizer.Source = Source
        Results = New List(Of Token)
        LineNumber = 0
        Using Reader As New StreamReader(Source.Filepath)
            Do Until Reader.EndOfStream
                Try
                    TokenizeLine(Reader.ReadLine())
                Catch ex As IndexOutOfRangeException
                    Throw New LocatedError("Line ended too soon", "A token wasn't finished at the end of the line", New Location(Source, LineNumber, Line.Length - 1, Line.Length))
                End Try
                LineNumber += 1
            Loop
        End Using
        SanitazeResults()
        Return Results
    End Function

    Public Function TokenizeExpression(Source As String, ParentFile As SourceFile) As IEnumerable(Of Token)
        Tokenizer.Line = Source
        Tokenizer.Source = ParentFile
        Results = New List(Of Token)
        LineNumber = 0
        Col = 0

        TokenizeExpression()
        Results.Add(New Token(TokenType.LINESTART, New Location(ParentFile, LineNumber - 1, 0, 0), 0))

        Return Results
    End Function

    'Sanitaze results
    Private Sub SanitazeResults()

        'Add a newline at the end, to help for the ATS parser
        Results.Add(New Token(TokenType.LINESTART, New Location(Source, LineNumber - 1, 0, 0), 0))

        'Remove unused newline (multiple newline folowings each others)
        Dim LastTokenWasANewLine As Boolean = True
        Dim Index As Integer = 1
        While Index < Results.Count

            If Results(Index).Type = TokenType.LINESTART Then
                If LastTokenWasANewLine Then
                    Results.RemoveAt(Index - 1)
                Else
                    Index += 1
                End If
                LastTokenWasANewLine = True
            Else
                Index += 1
                LastTokenWasANewLine = False
            End If

        End While

        'Remove newline after comma
        Dim LastTokenWasAComma As Boolean = False
        Index = 0
        While Index < Results.Count

            If LastTokenWasAComma AndAlso Results(Index).Type = TokenType.LINESTART Then
                Results.RemoveAt(Index)
            Else
                LastTokenWasAComma = Results(Index).Type = TokenType.SYMBOL_COMMA
                Index += 1
            End If

        End While

    End Sub


    ' Utils for line tokenizer
    Private Col As Integer
    Private Line As String
    Private ReadOnly Property CurrentChar As Char
        Get
            Return If(Col < Line.Length, Line(Col), Nothing)
        End Get
    End Property
    Private ReadOnly Property NextChar As Char
        Get
            Return If(Col + 1 < Line.Length, Line(Col + 1), Nothing)
        End Get
    End Property


    Private Sub AdvanceChar(Optional Count As Integer = 1)
        Col += Count
    End Sub

    Private StartCol As Integer
    Private Sub SaveCol()
        StartCol = Col
    End Sub
    Private Function LocationFromSave() As Location
        If Col = StartCol Then
            Return New Location(Source, LineNumber, StartCol, Col + 1)
        Else
            Return New Location(Source, LineNumber, StartCol, Col)
        End If
    End Function
    Private Function LocationFromChar() As Location
        Return New Location(Source, LineNumber, Col, Col + 1)
    End Function

    Private Sub AddToken(Type As TokenType, Location As Location)
        Results.Add(New Token(Type, Location))
    End Sub
    Private Sub AddToken(Type As TokenType, Value As Object, Location As Location)
        Results.Add(New Token(Type, Location, Value))
    End Sub

    Private Sub TokenizeLine(Line As String)

        'Skip empty and comment lines
        Dim StrippedLine As String = Line.Trim()
        If StrippedLine = "" OrElse StrippedLine.StartsWith("//") Then
            Exit Sub
        End If

        'Prepare data
        Col = 0
        Tokenizer.Line = Line

        'Get LineStart token
        SaveCol()
        Dim Indentation As Integer = 0
        While CurrentChar = vbTab
            Indentation += 1
            AdvanceChar()
        End While
        Results.Add(New Token(TokenType.LINESTART, LocationFromSave(), Indentation))

        '@preprocessor
        If CurrentChar = "@"c Then
            AdvanceChar()
            Dim Word As New StringBuilder()
            If Not VALID_TEXT_CHARS.Contains(CurrentChar) Then
                Throw New SyntaxError("A directive name was expected here.", LocationFromChar())
            End If
            SaveCol()
            While VALID_TEXT_CHARS.Contains(CurrentChar)
                Word.Append(CurrentChar)
                AdvanceChar()
            End While
            If Not CAPI.DoKeepLineFromPlatformName(Word.ToString(), LocationFromSave()) Then
                Exit Sub
            End If
            While Char.IsWhiteSpace(CurrentChar)
                AdvanceChar()
                Continue While
            End While
        End If

        'Source line
        If CurrentChar = "$"c Then
            AdvanceChar()
            SaveCol()
            Dim Source As String = Line.Substring(Col, Line.Length - Col).Trim
            If Source.StartsWith("include") Then
                Col += "include".Length
                Results.Add(New Token(TokenType.KEYWORD_INCLUDE, LocationFromSave()))
            Else
                Results.Add(New Token(TokenType.SOURCE_LINE, New Location(Tokenizer.Source, LineNumber, StartCol, Line.Length - Col), Source))
                Exit Sub
            End If
        End If

        TokenizeExpression()

    End Sub

    Private Sub TokenizeExpression()

        'Loop trought chars to create tokens
        While Not CurrentChar = Nothing

            'At this point CurrentToken as not been analysed

            'Skiped caracters
            If CurrentChar = " "c OrElse CurrentChar = vbTab Then
                AdvanceChar()
                Continue While
            End If

            'Number tokens (int, float)
            If DIGITS.Contains(CurrentChar) Then

                SaveCol()
                Dim Number As String = ""
                Dim HasPoint As Boolean = False
                While DIGITS_WITH_POINT.Contains(CurrentChar)
                    If CurrentChar = "."c Then
                        If HasPoint Then
                            Exit While
                        Else
                            HasPoint = True
                        End If
                    End If
                    Number &= CurrentChar
                    AdvanceChar()
                End While

                Dim AddDotToken As Boolean = False
                Dim Loc As Location = LocationFromSave()

                Try
                    If HasPoint Then
                        AddToken(TokenType.VAL_FLOAT, Convert.ToDouble(Number.Replace(".", ",")), Loc)
                    Else
                        AddToken(TokenType.VAL_INT, Convert.ToInt32(Number), Loc)
                    End If
                Catch ex As FormatException
                    Throw New LocatedError("Invalid number", $"The number ""{Number}"" is not a valid one.", Loc)
                End Try

                If AddDotToken Then
                    Dim DotLoc As Location = Loc
                    DotLoc.FromCol = DotLoc.ToCol
                    DotLoc.ToCol += 1
                    AddToken(TokenType.SYMBOL_POINT, DotLoc)
                End If

                Continue While

            End If

            'Text & keywords tokens
            If VALID_TEXT_CHARS.Contains(CurrentChar) Then

                SaveCol()
                Dim Text As String = ""
                While VALID_TEXT_CHARS_AFTER_FIRST_LETTER.Contains(CurrentChar)
                    Text &= CurrentChar
                    AdvanceChar()
                End While

                Dim Loc As Location = LocationFromSave()

                Select Case Text.ToLower()
                    Case "import"
                        AddToken(TokenType.KEYWORD_IMPORT, Loc)
                    Case "use"
                        AddToken(TokenType.KEYWORD_USE, Loc)
                    Case "as"
                        AddToken(TokenType.KEYWORD_AS, Loc)
                    Case "export"
                        AddToken(TokenType.KEYWORD_EXPORT, Loc)
                    Case "func"
                        AddToken(TokenType.KEYWORD_FUNC, Loc)
                    Case "let"
                        AddToken(TokenType.KEYWORD_LET, Loc)
                    Case "const"
                        AddToken(TokenType.KEYWORD_CONST, Loc)
                    Case "true"
                        AddToken(TokenType.VAL_BOOL, True, Loc)
                    Case "false"
                        AddToken(TokenType.VAL_BOOL, False, Loc)
                    Case "and"
                        AddToken(TokenType.KEYWORD_AND, Loc)
                    Case "or"
                        AddToken(TokenType.KEYWORD_OR, Loc)
                    Case "not"
                        AddToken(TokenType.KEYWORD_NOT, Loc)
                    Case "panic"
                        AddToken(TokenType.KEYWORD_PANIC, Loc)
                    Case "record"
                        AddToken(TokenType.KEYWORD_RECORD, Loc)
                    Case "while"
                        AddToken(TokenType.KEYWORD_WHILE, Loc)
                    Case "for"
                        AddToken(TokenType.KEYWORD_FOR, Loc)
                    Case "continue"
                        AddToken(TokenType.KEYWORD_CONTINUE, Loc)
                    Case "break"
                        AddToken(TokenType.KEYWORD_BREAK, Loc)
                    Case "if"
                        AddToken(TokenType.KEYWORD_IF, Loc)
                    Case "else"
                        AddToken(TokenType.KEYWORD_ELSE, Loc)
                    Case "elseif"
                        AddToken(TokenType.KEYWORD_ELSEIF, Loc)
                    Case "return"
                        AddToken(TokenType.KEYWORD_RETURN, Loc)
                    Case "from"
                        AddToken(TokenType.KEYWORD_FROM, Loc)
                    Case "to"
                        AddToken(TokenType.KEYWORD_TO, Loc)
                    Case "in"
                        AddToken(TokenType.KEYWORD_IN, Loc)
                    Case "struct"
                        AddToken(TokenType.KEYWORD_STRUCT, Loc)
                    Case "class"
                        AddToken(TokenType.KEYWORD_CLASS, Loc)
                    Case "new"
                        AddToken(TokenType.KEYWORD_NEW, Loc)
                    Case "relation"
                        AddToken(TokenType.KEYWORD_RELATION, Loc)
                    Case Else
                        AddToken(TokenType.TEXT, Text, Loc)
                End Select

                Continue While


            End If

            'String
            If CurrentChar = """"c Then

                SaveCol()
                AdvanceChar() 'Skip first quote
                Dim Str As String = ""
                While Not CurrentChar = """"c
                    If CurrentChar = Nothing Then
                        Throw New LocatedError("String not closed", "The string was not closed before the end of the line.", LocationFromSave())
                    End If
                    Str &= CurrentChar
                    AdvanceChar()
                End While
                AdvanceChar() 'Skip last quote

                AddToken(TokenType.VAL_STRING, Str, LocationFromSave())

                Continue While

            End If

            'Formated string
            If CurrentChar = "'"c Then

                SaveCol()
                AdvanceChar() 'Skip first quote
                Dim Str As String = ""
                While Not CurrentChar = "'"c
                    If CurrentChar = Nothing Then
                        Throw New LocatedError("String not closed", "The string was not closed before the end of the line.", LocationFromSave())
                    End If
                    If CurrentChar = "\"c Then
                        AdvanceChar()
                        If CurrentChar = Nothing Then
                            Throw New LocatedError("String not closed", "The string was not closed before the end of the line.", LocationFromSave())
                        End If
                        Str &= "\" & CurrentChar
                    Else
                        Str &= CurrentChar
                    End If
                    AdvanceChar()
                End While
                AdvanceChar() 'Skip last quote

                AddToken(TokenType.VAL_FORMATED_STRING, Str, LocationFromSave())

                Continue While

            End If

            'Final check: characters tokens
            Select Case CurrentChar

                Case "+"c
                    AddToken(TokenType.SYMBOL_PLUS, LocationFromChar())
                Case "-"c
                    AddToken(TokenType.SYMBOL_MINUS, LocationFromChar())
                Case "*"c
                    AddToken(TokenType.SYMBOL_MULTIPLICATE, LocationFromChar())
                Case "/"c
                    AddToken(TokenType.SYMBOL_DIVIDE, LocationFromChar())
                Case "%"c
                    AddToken(TokenType.SYMBOL_MODULO, LocationFromChar())
                Case "."c
                    AddToken(TokenType.SYMBOL_POINT, LocationFromChar())
                Case ","c
                    AddToken(TokenType.SYMBOL_COMMA, LocationFromChar())
                Case "<"c
                    If NextChar = "="c Then
                        SaveCol()
                        AdvanceChar()
                        AddToken(TokenType.SYMBOL_LESSTHANEQUAL, LocationFromSave())
                    Else
                        AddToken(TokenType.SYMBOL_LESSTHAN, LocationFromChar())
                    End If
                Case ">"c
                    If NextChar = "="c Then
                        SaveCol()
                        AdvanceChar()
                        AddToken(TokenType.SYMBOL_GREATERTHANEQUAL, LocationFromSave())
                    Else
                        AddToken(TokenType.SYMBOL_GREATERTHAN, LocationFromChar())
                    End If
                Case "["c
                    AddToken(TokenType.SYMBOL_LEFT_BRACKETS, LocationFromChar())
                Case "]"c
                    AddToken(TokenType.SYMBOL_RIGHT_BRACKETS, LocationFromChar())
                Case "("c
                    AddToken(TokenType.SYMBOL_LEFT_PARENTHESIS, LocationFromChar())
                Case ")"c
                    AddToken(TokenType.SYMBOL_RIGHT_PARENTHESIS, LocationFromChar())
                Case "{"c
                    AddToken(TokenType.SYMBOL_LEFT_BRACE, LocationFromChar())
                Case "}"c
                    AddToken(TokenType.SYMBOL_RIGHT_BRACE, LocationFromChar())
                Case ":"c
                    AdvanceChar()
                    Dim Loc As Location = LocationFromChar()
                    If CurrentChar = ":"c Then
                        Loc.ToCol += 1
                        AddToken(TokenType.OP_MODULE_RESOLVER, Loc)
                    Else
                        Col -= 1
                        AddToken(TokenType.SYMBOL_COLON, Loc)
                    End If
                Case "="c
                    AddToken(TokenType.SYMBOL_EQUAL, LocationFromChar())
                Case "@"c
                    AddToken(TokenType.SYMBOL_AT, LocationFromChar())
                Case Else
                    'Final error: unexpected character
                    Throw New LocatedError("Unexpected character", $"The following character was not expected : ""{CurrentChar}""", LocationFromChar())

            End Select

            'Goto next character, only SELECT CASE finish here, other tokens pass by using CONTINUE WHILE
            AdvanceChar()

        End While
    End Sub


End Module
