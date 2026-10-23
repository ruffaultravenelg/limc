Imports limc.Lexer

Namespace AST
    Friend Class FileParser

        ' ==== SHARED ====
        Public Shared Function Parse(Tokens As IEnumerable(Of Lexer.Token), AbsoluteFilePath As String) As FileNode
            Dim Parser As New FileParser(Tokens)
            Return Parser.Process(AbsoluteFilePath)
        End Function

        ' ==== CLASS ====

        Private Tokens As IEnumerable(Of Token)
        Private TokenIndex As Integer = -1
        Private CurrentToken As Token = Nothing

        Private Sub New(Tokens As IEnumerable(Of Token))
            Me.Tokens = Tokens
        End Sub

        ' Parsing at file level
        Public Function Process(AbsoluteFilepath As String) As FileNode
            Advance()

            Dim ImportNodes As New List(Of ImportFileConstruct)
            Dim Functions As New List(Of FunctionConstruct)

            ' Trying to parse constructs
            While TokenIndex < Tokens.Count - 1
                CheckNewline(0)
                Advance()

                ' Imports
                Dim Imp = TryImport()
                If Imp IsNot Nothing Then
                    ImportNodes.Add(Imp)
                    Continue While
                End If

                ' Functions
                Dim Func = TryFunction()
                If Func IsNot Nothing Then
                    Functions.Add(Func)
                    Continue While
                End If

                Throw New UnexpectedTokenException("P09", CurrentToken.Type, CurrentToken.Location)

            End While

            Return New FileNode(AbsoluteFilepath, ImportNodes, Functions)

        End Function

        ' ==== HELPING FUNCTIONS =====

        ' Advance to next token
        Private Sub Advance()
            TokenIndex += 1
            If TokenIndex >= Tokens.Count Then
                Throw New SyntaxException("P01", "Unexpected end of file.", Tokens.Last.Location)
            End If
            CurrentToken = Tokens(TokenIndex)
        End Sub

        Private ReadOnly Property LastTok As Token
            Get
                If TokenIndex < 1 Then
                    Throw New InternalException("Cannot access last token")
                Else
                    Return Tokens(TokenIndex - 1)
                End If
            End Get
        End Property

        Private _LocationStack As New Stack(Of Location)
        Private Sub PushLocation()
            _LocationStack.Push(CurrentToken.Location)
        End Sub
        Private Function PopLocation() As Location
            Return _LocationStack.Pop()
        End Function
        ' Pop location and add it with the one of the last token
        Private Function PopResolveLocation() As Location
            If TokenIndex - 1 < 0 Then
                Throw New InternalException("Cannot add with last token location: there is no last token")
            End If
            Return Tokens(TokenIndex - 1).Location + PopLocation()
        End Function

        Private Sub CheckNewline(MaximumIndentationLevel As Integer)
            If Not CurrentToken.Type = TokenType.LINESTART Then
                Throw New UnexpectedTokenException("P02", CurrentToken, TokenType.LINESTART, CurrentToken.Location)
            End If
            If DirectCast(CurrentToken.Value, Integer) > MaximumIndentationLevel Then
                Throw New WrongIndentationExcepetion("P03", CurrentToken.Value, MaximumIndentationLevel, CurrentToken.Location)
            End If
        End Sub

        Private Sub CheckToken(Code As String, ExpectedToken As TokenType)
            If Not CurrentToken.Type = ExpectedToken Then
                Throw New UnexpectedTokenException(Code, CurrentToken, ExpectedToken, CurrentToken.Location)
            End If
        End Sub
        Private Sub CheckToken(Code As String, ExpectedToken As TokenType, ExpectedThing As String)
            If Not CurrentToken.Type = ExpectedToken Then
                Throw New UnexpectedTokenException(Code, CurrentToken, ExpectedThing, CurrentToken.Location)
            End If
        End Sub

        ' ==== CONSTRUCTS (returns Nothing if not the right construct) ====
        Private Function TryImport() As ImportFileConstruct

            ' Check "import" keyword
            If Not CurrentToken.Type = TokenType.KEYWORD_IMPORT Then
                Return Nothing
            End If
            PushLocation()
            Advance()

            ' Get filename
            CheckToken("P15", TokenType.RAW_STR)
            Dim Filename As String = CurrentToken.Value
            Advance()

            ' Create & return node
            Return New ImportFileConstruct(Filename, PopResolveLocation())

        End Function

        Private Function TryFunction() As FunctionConstruct

            ' Check "func" keyword
            If Not CurrentToken.Type = TokenType.KEYWORD_FUNC Then
                Return Nothing
            End If
            PushLocation()
            Advance()

            ' Get name
            CheckToken("P04", TokenType.IDENTIFIER, "function name")
            Dim Name As String = CurrentToken.Value
            Advance()

            ' Get arguemnts
            Dim Arguments As IEnumerable(Of ArgumentNode) = GetOptionnaListOf(AddressOf GetArgument, TokenType.OPENING_PARENTHESIS, TokenType.CLOSING_PARENTHESIS)

            ' Get return type
            Dim ReturnType As GenericTypeNode = Nothing
            If CurrentToken.Type = TokenType.COLON Then
                Advance()
                ReturnType = GetTypeNode()
            End If

            ' Get body
            Dim Body = GetBody(1)

            ' Create & return function
            Return New FunctionConstruct(Name, {}, Arguments, ReturnType, Body, PopResolveLocation())

        End Function

        Private Function GetArgument() As ArgumentNode

            CheckToken("P05", TokenType.IDENTIFIER, "argument name")
            Dim Name As String = CurrentToken.Value
            PushLocation()
            Advance()

            CheckToken("P06", TokenType.COLON)
            Advance()

            Dim Type = GetTypeNode()

            Return New ArgumentNode(Name, Type, PopResolveLocation())

        End Function

        ' ==== STATEMENTS ====
        ' List of all functions that parse a statement
        Private ReadOnly STATEMENT_PARSING_FUNCTIONS As IEnumerable(Of Func(Of Integer, StatementNode)) = {
            AddressOf GetDeclareVariableInferType
        }

        ' Get a list of statement (level = the level of the futur statements)
        Private Function GetBody(Level As Integer) As BlockStatement
            Dim Statements As New List(Of StatementNode)
            While TokenIndex < Tokens.Count - 1
                CheckNewline(Level)
                If DirectCast(CurrentToken.Value, Integer) < Level Then
                    Exit While
                End If
                Advance()

                ' Loop over all statement parsing function, add to statements[] the result that isn't null
                Dim Found As Boolean = False
                For Each Parsing_Func In STATEMENT_PARSING_FUNCTIONS
                    Dim Res = Parsing_Func(Level)
                    If Res IsNot Nothing Then
                        Statements.Add(Res)
                        Found = True
                        Exit For
                    End If
                Next

                If Not Found Then
                    Throw New UnexpectedTokenException("P10", "An statement/instruction was expected here.", CurrentToken.Location)
                End If

            End While
            Return New BlockStatement(Statements)
        End Function

        Private Function GetDeclareVariableInferType(Level As Integer) As StatementNode

            ' Skip if not "let"
            If Not CurrentToken.Type = TokenType.KEYWORD_LET Then
                Return Nothing
            End If
            PushLocation()
            Advance()

            ' Get variable name
            CheckToken("P11", TokenType.IDENTIFIER, "variable name")
            Dim VariableName As String = CurrentToken.Value
            Advance()

            ' Get type
            Dim VariableType As TypeNode = Nothing
            If CurrentToken.Type = TokenType.COLON Then
                Advance()
                VariableType = GetTypeNode()
            End If

            ' Check = & get value
            If Not CurrentToken.Type = TokenType.EQUAL Then
                Throw New SyntaxException("P12", "A variable must always have a value. Therefore, you must initialize it with a value using the = sign right after.", CurrentToken.Location)
            End If
            Advance()
            Dim VariableValue As ExpressionNode = GetExpression()

            ' Create node
            If VariableType Is Nothing Then
                Return New DeclareVariableInferTypeStatement(VariableName, VariableValue, PopResolveLocation())
            Else
                ' TODO: Add declarevariablewithtypestatement
                Throw New NotImplementedException()
            End If

        End Function

        ' ==== EXPRESSIONS ====
        Private Function GetExpression() As ExpressionNode
            Return GetIndexPropertieCall()
        End Function

        ' Get elm[elm] OR elm.elm OR elm(elm, elm, ...)
        Private Function GetIndexPropertieCall() As ExpressionNode
            Dim Left As ExpressionNode = GetLiteral()

            While True
                Select Case CurrentToken.Type
                    Case TokenType.OPENING_BRACKET
                        'elm[elm]
                        Throw New NotImplementedException()

                    Case TokenType.POINT
                        'elm.elm
                        Throw New NotImplementedException()

                    Case TokenType.OPENING_PARENTHESIS
                        'elm(elm, elm, ...)
                        Advance()

                        Dim PassedArguments As New List(Of ExpressionNode)
                        While True
                            PassedArguments.Add(GetExpression())
                            If CurrentToken.Type = TokenType.COMMA Then
                                Advance()
                            ElseIf CurrentToken.Type = TokenType.CLOSING_PARENTHESIS Then
                                Advance()
                                Exit While
                            Else
                                Throw New UnexpectedTokenException("P13", CurrentToken, "comma or a closing parenthesis", CurrentToken.Location)
                            End If
                        End While

                        Left = New ProcedureCallExpression(Left.Location + LastTok.Location, Left, PassedArguments)

                    Case Else
                        Exit While
                End Select

            End While

            Return Left
        End Function

        ' Literal = last expression of the chain, smaller unit possible
        Private Function GetLiteral() As ExpressionNode
            Select Case CurrentToken.Type
                Case TokenType.INT
                    Advance()
                    Return New IntegerExpression(LastTok.Value, LastTok.Location)

                Case TokenType.FLOAT
                    Advance()
                    Return New FloatExpression(LastTok.Value, LastTok.Location)

                Case TokenType.RAW_STR
                    Advance()
                    Return New RawStringExpression(LastTok.Value, LastTok.Location)

                Case TokenType.FORMATTED_STR
                    Advance()
                    Return New FormattedStringExpression(LastTok.Value, LastTok.Location)

                Case TokenType.BOOL
                    Advance()
                    Return New BooleanExpression(LastTok.Value, LastTok.Location)

                Case TokenType.OPENING_PARENTHESIS
                    Advance()
                    Dim Expr As ExpressionNode = GetExpression()
                    CheckToken("P14", TokenType.CLOSING_PARENTHESIS)
                    Advance()
                    Return Expr

                Case TokenType.IDENTIFIER
                    Advance()
                    If CurrentToken.Type = TokenType.LT Then
                        PushLocation()
                        Dim ElementName As String = LastTok.Value
                        Dim PassedGenerics As New List(Of TypeNode)
                        If Not CurrentToken.Type = TokenType.GT Then
                            While True
                                PassedGenerics.Add(GetTypeNode())
                                If CurrentToken.Type = TokenType.COMMA Then
                                    Advance()
                                ElseIf CurrentToken.Type = TokenType.GT Then
                                    Exit While
                                Else
                                    Throw New SyntaxException("P15", "A comma or a > was expected here.", CurrentToken.Location)
                                End If
                            End While
                        End If
                        Advance()
                        If PassedGenerics.Any Then
                            Return New GenericElementExpression(ElementName, PassedGenerics, PopResolveLocation())
                        Else
                            Return New ElementExpression(ElementName, PopResolveLocation())
                        End If
                    Else
                        Return New ElementExpression(LastTok.Value, LastTok.Location)
                    End If

                Case Else
                    Throw New UnexpectedTokenException("P13", CurrentToken)

            End Select
        End Function

        ' ==== TYPE ====
        Private Function GetTypeNode() As GenericTypeNode

            'name<generic, generic, ...>

            CheckToken("P06", TokenType.IDENTIFIER, "Type name")
            Dim Name As String = CurrentToken.Value
            PushLocation()
            Advance()

            Dim PassedGenerics As New List(Of GenericTypeNode)
            If CurrentToken.Type = TokenType.LT Then
                Advance()
                If Not CurrentToken.Type = TokenType.GT Then
                    Do
                        PassedGenerics.Add(GetTypeNode()) ' Get new typenode

                        If CurrentToken.Type = TokenType.COMMA Then
                            Advance()
                        ElseIf CurrentToken.Type = TokenType.GT Then
                            Exit Do
                        Else
                            Throw New SyntaxException("P07", "A comma or a > was expected here.", CurrentToken.Location)
                        End If
                    Loop
                End If
                Advance()
            End If

            Return New GenericTypeNode(Name, PassedGenerics, PopResolveLocation())

        End Function

        ' ==== PARSING HELPING ====
        Private Function GetListOf(Of T)(ParsingFunc As Func(Of T)) As IEnumerable(Of T)
            Dim Args As New List(Of T)
            Do
                Args.Add(ParsingFunc())

                If CurrentToken.Type = TokenType.COMMA Then
                    Advance()
                Else
                    Exit Do
                End If
            Loop
            Return Args
        End Function
        Private Function GetOptionnaListOf(Of T)(ParsingFunc As Func(Of T), StartingToken As TokenType, EndingToken As TokenType) As IEnumerable(Of T)

            ' No starting token -> empty list
            If Not CurrentToken.Type = StartingToken Then
                Return {}
            End If
            Advance()

            ' Starting token followed by ending token -> empty list
            If CurrentToken.Type = EndingToken Then
                Return {}
            End If

            ' Get list
            Dim Args = GetListOf(Of T)(ParsingFunc)

            ' Check ending token
            If Not CurrentToken.Type = EndingToken Then
                Throw New SyntaxException("P08", $"A comma or a {TokenTypeToEnglish(EndingToken)} was expected here.", CurrentToken.Location)
            End If
            Advance()

            Return Args

        End Function

    End Class
End Namespace