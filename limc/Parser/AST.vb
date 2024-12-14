Public Class AST

    'Properties
    Private Lines As IteratorAdapter(Of SourceLine)
    Private Tokens As IteratorAdapter(Of Token)

    'Constructor
    Private Sub New(Lines As IEnumerable(Of SourceLine))
        Me.Lines = New IteratorAdapter(Of SourceLine)(Lines)
    End Sub

    'Parser entry point
    Public Shared Function GenerateAST(Lines As IEnumerable(Of SourceLine)) As FileAST
        Dim Parser As New AST(Lines)
        Return Parser.Parse()
    End Function

    'Parse
    Private Function Parse() As FileAST

        'Create result
        Dim Result As New FileAST()

        'Get constructs
        Dim First As Boolean = True
        While Lines.HasNext

            'Goto next line
            If Not First Then
                Lines.Next()
            End If
            First = False

            'Indentation ot a 0
            If Lines.Current.Indentation <> 0 Then
                Throw New SyntaxException("Indetation must be 0", Lines.Current.Location)
            End If

            'Empty line
            If Lines.Current.Tokens.Count = 0 Then
                Continue While
            End If

            'Peak line first token
            EnterLine()

            'Import
            If Tokens.Current.Type = Token.TokenType.KEYWORD_IMPORT Then
                Result.Import.Add(GetImport())
                Continue While
            End If

            'Functions
            If Tokens.Current.Type = Token.TokenType.KEYWORD_FUNC Then
                Result.Functions.Add(GetFunction())
                Continue While
            End If

            'Unknown lines
            Throw New SyntaxException("Unexpected token, expected a construct", Tokens.Current.Location)

        End While

        'Return
        Return Result

    End Function

    'Create token iterator from the current line
    Private Sub EnterLine()
        Tokens = New IteratorAdapter(Of Token)(Lines.Current.Tokens)
    End Sub

    'Get generic types
    Private Function GetGenericTypes() As List(Of String)

        'Create result
        Dim Result As New List(Of String)

        'Content ?
        If Not Tokens.HasNext Then
            Return Result
        End If

        ' <
        Tokens.Next()
        If Not Tokens.Current.Type = Token.TokenType.OPERATOR_LESSTHAN Then
            Return Result
        End If

        ' type
        While True

            'Name
            If Not Tokens.Current.Type = Token.TokenType.WORD Then
                Throw New SyntaxException("A Generic Type name was expected here", Tokens.Current.Location)
            End If
            Result.Add(Tokens.Current.Value)
            Tokens.Next()

            '>
            If Tokens.Current.Type = Token.TokenType.OPERATOR_MORETHAN Then
                Exit While
            End If

            ',
            If Tokens.Current.Type = Token.TokenType.SYNTAX_COMMA Then
                Tokens.Next()
                Continue While
            End If

            'Unexpected token
            Throw New SyntaxException("Unexpected token, expected a comma or a closing angle bracket", Tokens.Current.Location)

        End While


        'Return result
        Return Result

    End Function

    'Get function
    Private Function GetFunction() As Source.Function

        'No other token
        If Not Tokens.HasNext Then
            Throw New SyntaxException("A function name was expected after the ""func"" keyword", Tokens.Current.Location)
        End If

        'Pass "func" keyword
        Tokens.Next()

        'Get name
        If Not Tokens.Current.Type = Token.TokenType.WORD Then
            Throw New SyntaxException("A function name was expected here", Tokens.Current.Location)
        End If
        Dim Name As String = Tokens.Current.Value

        'Generic types
        Dim GenericTypes As IEnumerable(Of String) = GetGenericTypes()

        '




    End Function

    'Get import
    Private Function GetImport() As Source.Import

        'No other token
        If Not Tokens.HasNext Then
            Throw New SyntaxException("A library or filename was expected after the ""import"" keyword", Tokens.Current.Location)
        End If

        'Advance "import"
        Tokens.Next()

        'Get filename
        Dim Filename As String
        Dim Library As Boolean

        If Tokens.Current.Type = Token.TokenType.WORD Then
            Filename = Tokens.Current.Value
            Library = True
        ElseIf Tokens.Current.Type = Token.TokenType.VALUE_STRING Then
            Filename = Tokens.Current.Value
            Library = False
        Else
            Throw New SyntaxException("Unexpected token : " & Tokens.Current.Type.ToString(), Tokens.Current.Location)
        End If

        'Get "as"
        Dim Naming As String = ""
        If Tokens.HasNext Then

            'Not "as" keyword
            If Not Tokens.Next().Type = Token.TokenType.KEYWORD_AS Then
                Throw New SyntaxException("""as"" keyword was expected here", Tokens.Current.Location)
            End If

            'Check name
            If Tokens.Next().Type = Token.TokenType.SYNTAX_DOT Then
                Naming = "."
            Else
                If Not Tokens.Current.Type = Token.TokenType.WORD Then
                    Throw New SyntaxException("A name was expected here", Tokens.Current.Location)
                End If
                Naming = Tokens.Current.Value
            End If

        End If

        'To much character
        If Tokens.HasNext Then
            Throw New SyntaxException("Nothing more was expected on this line", Tokens.Next().Location)
        End If

        'Create import
        Return New Source.Import(Lines.Current.Location, Filename, Library, Naming)

    End Function


End Class
