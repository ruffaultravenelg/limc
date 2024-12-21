Public Class AST

    'Constants
    Private ReadOnly CONSTRUCTS As IEnumerable(Of Func(Of ConstructNode)) = {AddressOf GetImport, AddressOf GetFunction}

    'Properties
    Private Tokens As IteratorAdapter(Of Token)

    'Constructor
    Private Sub New(Tokens As IEnumerable(Of Token))
        Me.Tokens = New IteratorAdapter(Of Token)(Tokens)
    End Sub

    'Get location
    Private Function LocationFrom(StartLocation As Location) As Location
        Return StartLocation + Tokens.Last.Location
    End Function

    'Parser entry point
    Public Shared Function GenerateAST(Lines As IEnumerable(Of SourceLine)) As FileAST

        'Empty lines
        If Lines.Count = 0 Then
            Return New FileAST()
        End If

        'Convert lines into list of tokens
        Dim Tokens As New List(Of Token)
        For Each Line As SourceLine In Lines
            Tokens.Add(New Token(Token.TokenType.LINESTART, Line.Indentation, Line.Location))
            Tokens.AddRange(Line.Tokens)
        Next

        'Add last token
        Tokens.Add(New Token(Token.TokenType.LINESTART, Lines.Last.Location))

        'Return
        Return GenerateAST(Tokens)

    End Function
    Public Shared Function GenerateAST(Lines As IEnumerable(Of Token)) As FileAST
        Dim Parser As New AST(Lines)
        Return Parser.Parse()
    End Function

    'Parse
    Private Function Parse() As FileAST

        'Create result
        Dim Result As New FileAST()

        'Get constructs
        While Tokens.HasNext

            'Start line
            If Not Tokens.Current.Type = Token.TokenType.LINESTART Then
                Throw New SyntaxException("Unexpected token, expected a line start", Tokens.Current.Location)
            End If

            'Get indentation
            Dim Indentation As Integer = Tokens.Current.Value
            If Indentation <> 0 Then
                Throw New SyntaxException("Indetation must be 0", Tokens.Current.Location)
            End If

            'Get constructs
            Tokens.Next()
            Dim Construct As ConstructNode = Nothing
            For Each ParsingFunction In CONSTRUCTS

                Try
                    Construct = ParsingFunction()
                    Exit For
                Catch ex As NotTheRightElement
                    Continue For
                End Try

            Next

            'Append or error
            If Construct Is Nothing Then
                Throw New SyntaxException("Unexpected token, expected a construct", Tokens.Current.Location)
            Else
                Result.AppendConstruct(Construct)
            End If

        End While

        'Return
        Return Result

    End Function

    'Get type
    Private Function GetAType() As Source.Type

        'Save location
        Dim StartLocation As Location = Tokens.Current.Location

        'Get name
        If Not Tokens.Current.Type = Token.TokenType.WORD Then
            Throw New SyntaxException("A type name was expected here.", Tokens.Current.Location)
        End If
        Dim Name As String = Tokens.Current.Value
        Tokens.Next()

        'If file is precised (io::file)
        Dim File As String = ""
        If Tokens.Current.Type = Token.TokenType.SYNTAX_DOUBLECOLON Then
            Tokens.Next()
            If Not Tokens.Current.Type = Token.TokenType.WORD Then
                Throw New SyntaxException("A type name was expected here.", Tokens.Current.Location)
            End If
            File = Name
            Name = Tokens.Current.Value
            Tokens.Next()
        End If

        'Get passed generic types
        Dim PassedGenericTypes As New List(Of Source.Type)
        If Tokens.Current.Type = Token.TokenType.OPERATOR_LESSTHAN Then
            Tokens.Next()
            If Not Tokens.Current.Type = Token.TokenType.OPERATOR_MORETHAN Then
                While True

                    'Get type
                    PassedGenericTypes.Add(GetAType())

                    'Check end
                    If Tokens.Current.Type = Token.TokenType.OPERATOR_MORETHAN Then
                        Tokens.Next()
                        Exit While
                    End If

                    'Check comma
                    If Tokens.Current.Type = Token.TokenType.SYNTAX_COMMA Then
                        Tokens.Next()
                    Else
                        Throw New SyntaxException("A comma or a '>' was expected here.", Tokens.Current.Location)
                    End If

                End While
            Else
                Tokens.Next()
            End If
        End If

        'Return
        If File = "" Then
            Return New Source.Type(LocationFrom(StartLocation), Name, PassedGenericTypes)
        Else
            Return New Source.FiledType(LocationFrom(StartLocation), File, Name, PassedGenericTypes)
        End If

    End Function

    'Get arguments
    Private Function GetArguments() As IEnumerable(Of Source.Argument)

        'Create result
        Dim Result As New List(Of Source.Argument)

        'If nothing
        If Not Tokens.Current.Type = Token.TokenType.SYNTAX_LEFT_PARENTHESIS Then
            Return Result
        End If

        'Skip '('
        Tokens.Next()

        'If empty
        If Tokens.Current.Type = Token.TokenType.SYNTAX_RIGHT_PARENTHESIS Then
            Tokens.Next()
            Return Result
        End If

        'Get arguments
        While True

            'Save location
            Dim StartLocation As Location = Tokens.Current.Location

            'Get name
            If Not Tokens.Current.Type = Token.TokenType.WORD Then
                Throw New SyntaxException("A name was expected here.", Tokens.Current.Location)
            End If
            Dim Name As String = Tokens.Current.Value

            'Skip name
            Tokens.Next()

            'Get type
            If Not Tokens.Current.Type = Token.TokenType.SYNTAX_COLON Then
                Throw New SyntaxException("A colon was expected here.", Tokens.Current.Location)
            End If
            Tokens.Next()
            Dim Type As Source.Type = GetAType()

            'Add
            Result.Add(New Source.Argument(LocationFrom(StartLocation), Name, Type))

            'Check end
            If Tokens.Current.Type = Token.TokenType.SYNTAX_RIGHT_PARENTHESIS Then
                Tokens.Next()
                Exit While
            End If

            'Check comma
            If Tokens.Current.Type = Token.TokenType.SYNTAX_COMMA Then
                Tokens.Next()
            Else
                Throw New SyntaxException("A comma or a ')' was expected here.", Tokens.Current.Location)
            End If

        End While

        'Return result
        Return Result

    End Function

    'Get generic types
    Private Function GetGenericTypes() As IEnumerable(Of Source.GenericType)

        'Create result
        Dim Result As New List(Of Source.GenericType)

        'If nothing
        If Not Tokens.Current.Type = Token.TokenType.OPERATOR_LESSTHAN Then
            Return Result
        End If

        'Skip "<"
        Tokens.Next()

        'If empty
        If Tokens.Current.Type = Token.TokenType.OPERATOR_MORETHAN Then
            Tokens.Next()
            Return Result
        End If

        'Get types
        While True

            'Get name
            If Not Tokens.Current.Type = Token.TokenType.WORD Then
                Throw New SyntaxException("A name was expected here.", Tokens.Current.Location)
            End If
            Dim Name As String = Tokens.Current.Value

            'Add
            Result.Add(New Source.GenericType(Tokens.Current.Location, Name))
            Tokens.Next()

            'Check end
            If Tokens.Current.Type = Token.TokenType.OPERATOR_MORETHAN Then
                Tokens.Next()
                Exit While
            End If

            'Check comma
            If Tokens.Current.Type = Token.TokenType.SYNTAX_COMMA Then
                Tokens.Next()
            Else
                Throw New SyntaxException("A comma or a '>' was expected here.", Tokens.Current.Location)
            End If

        End While

        'Return result
        Return Result

    End Function

    'Get function
    Private Function GetFunction() As Source.Function

        'Save start location
        Dim StartLocation As Location = Tokens.Current.Location

        'Check "func" keyword
        If Not Tokens.Current.Type = Token.TokenType.KEYWORD_FUNC Then
            Throw New NotTheRightElement()
        End If
        Tokens.Next()

        'Get name
        If Not Tokens.Current.Type = Token.TokenType.WORD Then
            Throw New SyntaxException("A name was expected here.", Tokens.Current.Location)
        End If
        Dim Name As String = Tokens.Current.Value
        Tokens.Next()

        'Get generic types
        Dim GenericTypes As IEnumerable(Of Source.GenericType) = GetGenericTypes()

        'Get arguments
        Dim Arguments As IEnumerable(Of Source.Argument) = GetArguments()

        'Get return type
        Dim ReturnType As Source.Type = Nothing
        If Tokens.Current.Type = Token.TokenType.SYNTAX_COLON Then
            Tokens.Next()
            ReturnType = GetAType()
        End If

        'Return function
        Return New Source.Function(LocationFrom(StartLocation), Name, GenericTypes, Arguments, ReturnType)

    End Function

    'Get import
    Private Function GetImport() As Source.Import

        'Save start location
        Dim StartLocation As Location = Tokens.Current.Location

        'Check "import" keyword
        If Not Tokens.Current.Type = Token.TokenType.KEYWORD_IMPORT Then
            Throw New NotTheRightElement()
        End If

        'Get path
        Tokens.Next()
        Dim Library As Boolean
        Dim Filename As String
        If Tokens.Current.Type = Token.TokenType.WORD Then
            Library = True
            Filename = Tokens.Current.Value
            Tokens.Next()
        ElseIf Tokens.Current.Type = Token.TokenType.VALUE_STRING Then
            Library = False
            Filename = Tokens.Current.Value
            Tokens.Next()
        ElseIf Tokens.Current.Type = Token.TokenType.VALUE_FSTRING Then
            Throw New SyntaxException("Formatted strings are not supported here, please use """, Tokens.Current.Location)
        Else
            Throw New SyntaxException("A name or a filepath was expected here.", Tokens.Current.Location)
        End If

        'Get "as" keyword
        Dim Naming As String = ""
        If Tokens.Current.Type = Token.TokenType.KEYWORD_AS Then
            Tokens.Next()
            If Tokens.Current.Type = Token.TokenType.SYNTAX_DOT Then
                Naming = "."
            ElseIf Tokens.Current.Type = Token.TokenType.WORD Then
                Naming = Tokens.Current.Value
            Else
                Throw New SyntaxException("A name was expected here.", Tokens.Current.Location)
            End If
            Tokens.Next()
        End If

        'Return
        Return New Source.Import(LocationFrom(StartLocation), Filename, Library, Naming)

    End Function

End Class
