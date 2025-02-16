Public Class AST

    'Constants
    Private ReadOnly CONSTRUCTS As IEnumerable(Of Func(Of ConstructNode)) = {AddressOf GetImport, AddressOf GetFunction, AddressOf GetInternalType, AddressOf GetStructure}
    Private ReadOnly FUNCTION_STATEMENTS As IEnumerable(Of Func(Of Integer, StatementNode)) = {AddressOf GetLet, AddressOf GetSourceStatement, AddressOf GetReturnStatement, AddressOf GetCallStatement}

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

            'Save location
            Tokens.Next()
            Dim StartLocation As Location = Tokens.Current.Location

            'Get exported
            Dim Exported As Boolean = False
            If Tokens.Current.Type = Token.TokenType.KEYWORD_EXPORT Then
                Exported = True
                Tokens.Next()
            End If

            'Save state
            Dim State As Integer = Tokens.SaveState()

            'Get constructs
            Dim Construct As ConstructNode = Nothing
            For Each ParsingFunction In CONSTRUCTS

                'Load state
                Tokens.LoadState(State)

                'Test parsing function
                Try
                    Construct = ParsingFunction()
                    Exit For
                Catch ex As NotTheRightElement
                    Continue For
                End Try

            Next

            'Append or error
            If Construct Is Nothing Then
                Throw New SyntaxException("Unexpected token, expected a construct", StartLocation)
            Else
                Construct.SetExported(Exported)
                Result.AppendConstruct(Construct)
            End If

        End While

        'Export all constructs if none of them is exported
        ConstructNode.HandleExports(Result.GetConstructs(Of ConstructNode)())

        'Return
        Return Result

    End Function

    'Get passed generic types
    Private Function GetPassedGenericTypes() As IEnumerable(Of Source.Type)

        'Create result
        Dim Result As New List(Of Source.Type)

        'Nothing to do
        If Not Tokens.Current.Type = Token.TokenType.OPERATOR_LESSTHAN Then
            Return Result
        End If
        Tokens.Next() 'Advance '<'

        'If empty '>'
        If Tokens.Current.Type = Token.TokenType.OPERATOR_MORETHAN Then
            Tokens.Next() 'skip '>'
            Return Result
        End If

        'Loop for passed types
        While True

            'Get type
            Result.Add(GetAType())

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
        Dim PassedGenericTypes As IEnumerable(Of Source.Type) = GetPassedGenericTypes()

        'fun<><>
        If Name = "fun" Then

            'Get return type
            Dim ReturnType As Source.Type = Nothing
            If Tokens.Current.Type = Token.TokenType.OPERATOR_LESSTHAN Then
                Tokens.Next()
                If Not Tokens.Current.Type = Token.TokenType.OPERATOR_MORETHAN Then
                    ReturnType = GetAType()
                    If Not Tokens.Current.Type = Token.TokenType.OPERATOR_MORETHAN Then
                        Throw New SyntaxException("A '>' was expected here.", Tokens.Current.Location)
                    End If
                    Tokens.Next()
                End If
            End If

            'Create type
            Return New Source.FunType(LocationFrom(StartLocation), PassedGenericTypes, ReturnType)

        End If

        'Return
        If File = "" Then
            Return New Source.Type(LocationFrom(StartLocation), Name, PassedGenericTypes)
        Else
            Return New Source.FiledType(LocationFrom(StartLocation), File, Name, PassedGenericTypes)
        End If

    End Function

    'Get arguments
    Private Function GetArguments() As IEnumerable(Of Source.KeyNameType)

        'Create result
        Dim Result As New List(Of Source.KeyNameType)

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
            Result.Add(New Source.KeyNameType(LocationFrom(StartLocation), Name, Type))

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

    'Get methods
    Private Function GetStructureMethods() As IEnumerable(Of Source.Function)

        'Create result
        Dim Methods As New List(Of Source.Function)

        'Get constructs
        While Tokens.HasNext

            'Start line
            If Not Tokens.Current.Type = Token.TokenType.LINESTART Then
                Throw New SyntaxException("Unexpected token, expected a line start", Tokens.Current.Location)
            End If

            'Get indentation
            Dim Indentation As Integer = Tokens.Current.Value
            If Indentation = 0 Then
                Exit While
            End If
            If Indentation <> 1 Then
                Throw New SyntaxException("Indetation must be 1 or 0", Tokens.Current.Location)
            End If

            'Save location
            Tokens.Next()
            Dim StartLocation As Location = Tokens.Current.Location

            'Get exported
            Dim Exported As Boolean = False
            If Tokens.Current.Type = Token.TokenType.KEYWORD_EXPORT Then
                Exported = True
                Tokens.Next()
            End If

            'Get method
            Try
                Dim Method As Source.Function = GetFunction(1)
                Method.SetExported(Exported)
                Methods.Add(Method)
            Catch ex As NotTheRightElement
                Throw New SyntaxException("A method was expected here", StartLocation)
            End Try

        End While

        'Export all constructs if none of them is exported
        ConstructNode.HandleExports(Methods)

        'Return methods
        Return Methods

    End Function

    'Get function
    Private Function GetFunction(Optional FunctionDefinitionIndentation As Integer = 0) As Source.Function

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
        Dim Arguments As IEnumerable(Of Source.KeyNameType) = GetArguments()

        'Get return type
        Dim ReturnType As Source.Type = Nothing
        If Tokens.Current.Type = Token.TokenType.SYNTAX_COLON Then
            Tokens.Next()
            ReturnType = GetAType()
        End If

        'Get body
        Dim Body As IEnumerable(Of StatementNode) = GetStatements(FUNCTION_STATEMENTS, FunctionDefinitionIndentation + 1)

        'Return function
        Return New Source.Function(LocationFrom(StartLocation), Name, GenericTypes, Arguments, ReturnType, Body)

    End Function

    'Get structure
    Private Function GetStructure() As Source.Struct

        'Save start location
        Dim StartLocation As Location = Tokens.Current.Location

        'Check "struct" keyword
        If Not Tokens.Current.Type = Token.TokenType.KEYWORD_STRUCT Then
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
        Dim Arguments As IEnumerable(Of Source.KeyNameType) = GetArguments()

        'No arguments
        If Arguments.Count = 0 Then
            Throw New SyntaxException("A structure need at least one fields.", Tokens.Current.Location)
        End If

        'Get methods
        Dim Methods As IEnumerable(Of Source.Function) = GetStructureMethods()

        'Return structure
        Return New Source.Struct(LocationFrom(StartLocation), Name, GenericTypes, Arguments, Methods)

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

    'Get internal type
    Private Function GetInternalType() As Source.InteralType

        'Save start location
        Dim StartLocation As Location = Tokens.Current.Location

        'Check "internal" word
        If Not (Tokens.Current.Type = Token.TokenType.WORD AndAlso Tokens.Current.Value.ToString().ToLower() = "internal") Then
            Throw New NotTheRightElement()
        End If

        'Check for "type" word
        Tokens.Next()
        If Not Tokens.Current.Type = Token.TokenType.WORD AndAlso Tokens.Current.Value.ToString().ToLower() = "type" Then
            Throw New NotTheRightElement()
        End If

        'Get type name
        Tokens.Next()
        If Not Tokens.Current.Type = Token.TokenType.WORD Then
            Throw New SyntaxException("A type name was expected here", Tokens.Current.Location)
        End If
        Dim Name As String = Tokens.Current.Value

        'Get generic types
        Tokens.Next()
        Dim GenericTypes As IEnumerable(Of Source.GenericType) = GetGenericTypes()

        'Return node
        Return New Source.InteralType(LocationFrom(StartLocation), Name, GenericTypes)

    End Function

    'Get statements
    Private Function GetStatements(AcceptedStatements As IEnumerable(Of Func(Of Integer, StatementNode)), CurrentIndentation As Integer) As IEnumerable(Of StatementNode)

        'Create result
        Dim Result As New List(Of StatementNode)

        'Get statements
        While Tokens.HasNext

            'Check tokentype
            If Not Tokens.Current.Type = Token.TokenType.LINESTART Then
                Throw New SyntaxException("Unexpected token, expected a line start", Tokens.Current.Location)
            End If

            'Check identation
            Dim Indentation As Integer = Tokens.Current.Value
            If Indentation < CurrentIndentation Then
                Exit While
            End If
            If Indentation > CurrentIndentation Then
                Throw New SyntaxException("Unexpected identation", Tokens.Current.Location)
            End If

            'Save token
            Tokens.Next()
            Dim StartLocation As Location = Tokens.Current.Location

            'Save state
            Dim State As Integer = Tokens.SaveState()

            'Get statement
            Dim Statement As StatementNode = Nothing
            For Each ParsingFunction In AcceptedStatements

                'Load state
                Tokens.LoadState(State)

                'Try parsing function
                Try
                    Statement = ParsingFunction(Indentation)
                    Exit For
                Catch ex As NotTheRightElement
                    Continue For
                End Try

            Next

            'Append or error
            If Statement Is Nothing Then
                Throw New SyntaxException("Unexpected token, expected a statement", StartLocation)
            Else
                Result.Add(Statement)
            End If

        End While

        'Return result
        Return Result

    End Function

    'Get let statement
    Private Function GetLet(CurrentIndentation As Integer)

        'Save start location
        Dim StartLocation As Location = Tokens.Current.Location

        'Check "let" keyword
        If Not Tokens.Current.Type = Token.TokenType.KEYWORD_LET Then
            Throw New NotTheRightElement()
        End If
        Tokens.Next()

        'Get name
        If Not Tokens.Current.Type = Token.TokenType.WORD Then
            Throw New SyntaxException("A variable name was expected here.", Tokens.Current.Location)
        End If
        Dim Name As String = Tokens.Current.Value
        Tokens.Next()

        'Get type
        Dim Type As Source.Type = Nothing
        If Tokens.Current.Type = Token.TokenType.SYNTAX_COLON Then
            Tokens.Next()
            Type = GetAType()
        End If

        'Get value
        Dim Value As ExpressionNode = Nothing
        If Tokens.Current.Type = Token.TokenType.OPERATOR_EQUAL Then
            Tokens.Next()
            Value = GetExpression()
        End If

        'Return node
        Return New Source.LetStatement(LocationFrom(StartLocation), Name, Type, Value)

    End Function

    'Get call statement
    Private Function GetCallStatement(CurrentIndentation As Integer)

        'Get expression
        Dim Expression As ExpressionNode = GetExpression()

        'If expression is not a call
        If TypeOf Expression IsNot Source.CallNode Then
            Throw New NotTheRightElement()
        End If

        'Return node
        Return New Source.CallStatement(Expression)

    End Function

    'Get source statement
    Private Function GetSourceStatement(CurrentIndentation As Integer)

        'Source sign ($)
        If Not Tokens.Current.Type = Token.TokenType.SYNTAX_SOURCE Then
            Throw New NotTheRightElement()
        End If

        'Get source
        Dim Source As String = Tokens.Current.Value
        Tokens.Next()

        'Return node
        Return New Source.SourceStatement(Tokens.Last.Location, Source)

    End Function

    'Get return statement
    Private Function GetReturnStatement(CurrentIndentation As Integer) As Source.ReturnStatement

        'Save location
        Dim StartLocation As Location = Tokens.Current.Location

        'If return keyword
        If Not Tokens.Current.Type = Token.TokenType.KEYWORD_RETURN Then
            Throw New NotTheRightElement()
        End If

        'Get value
        Tokens.Next()
        Dim Value As ExpressionNode = GetExpression()

        'Create node
        Return New Source.ReturnStatement(LocationFrom(StartLocation), Value)

    End Function

    'Get expression
    Private Function GetExpression() As ExpressionNode
        Return GetCallChildIdx()
    End Function

    'Get call / child / index
    Private Function GetCallChildIdx() As ExpressionNode

        'Get target
        Dim Target As ExpressionNode = GetFactor()

        'Some goofy loop
        While True
            Select Case Tokens.Current.Type

                Case Token.TokenType.SYNTAX_LEFT_PARENTHESIS 'CALL

                    'Empty lol
                    Tokens.Next()
                    If Tokens.Current.Type = Token.TokenType.SYNTAX_RIGHT_PARENTHESIS Then
                        Tokens.Next()
                        Return New Source.CallNode(LocationFrom(Target.Location), Target, {})
                    End If

                    'Values
                    Dim PassedArguments As New List(Of ExpressionNode)
                    While True

                        'Get argument
                        PassedArguments.Add(GetExpression())

                        'End
                        If Tokens.Current.Type = Token.TokenType.SYNTAX_RIGHT_PARENTHESIS Then
                            Tokens.Next()
                            Exit While
                        End If

                        'coma
                        If Tokens.Current.Type = Token.TokenType.SYNTAX_COMMA Then
                            Tokens.Next()
                            Continue While
                        End If

                        'Error
                        Throw New SyntaxException("The characters ')' or ',' were expected here.", Tokens.Current.Location)

                    End While

                    'Create node
                    Target = New Source.CallNode(LocationFrom(Target.Location), Target, PassedArguments)


                Case Token.TokenType.SYNTAX_DOT 'CHILD

                    'Pass dot
                    Tokens.Next()

                    'Not a word
                    If Not Tokens.Current.Type = Token.TokenType.WORD Then
                        Throw New SyntaxException("A propertie name was expected here", Tokens.Current.Location)
                    End If

                    'Get word
                    Dim Propertie As String = Tokens.Current.Value
                    Tokens.Next()

                    'Generic types ?
                    Dim GenericTypes As IEnumerable(Of Source.Type) = GetPassedGenericTypes()

                    'Create node
                    If GenericTypes.Count = 0 Then
                        Target = New Source.ChildNode(LocationFrom(Target.Location), Target, Propertie)
                    Else
                        Target = New Source.GenericChildNode(LocationFrom(Target.Location), Target, Propertie, GenericTypes)
                    End If

                Case Else 'END
                    Exit While

            End Select
        End While

        'Return this
        Return Target

    End Function

    'Get factor
    Private Function GetFactor() As ExpressionNode

        'Save first token
        Dim FirstToken As Token = Tokens.Current

        'Integer
        If Tokens.Current.Type = Token.TokenType.VALUE_INT Then
            Tokens.Next()
            Return New Source.IntNode(FirstToken.Location, FirstToken.Value)
        End If

        'String
        If Tokens.Current.Type = Token.TokenType.VALUE_STRING Then
            Tokens.Next()
            Return New Source.StrNode(FirstToken.Location, FirstToken.Value)
        End If

        'Element
        If Tokens.Current.Type = Token.TokenType.WORD Then

            'Get name
            Dim File As String = ""
            Dim Value As String = FirstToken.Value
            Tokens.Next()

            'If the name was a library name
            If Tokens.Current.Type = Token.TokenType.SYNTAX_DOUBLECOLON Then
                Tokens.Next()
                If Not Tokens.Current.Type = Token.TokenType.WORD Then
                    Throw New SyntaxException("A element name was expected here", Tokens.Current.Location)
                End If
                File = Value
                Value = Tokens.Current.Value
                Tokens.Next()
            End If

            'If there is NO generic types
            If Not Tokens.Current.Type = Token.TokenType.OPERATOR_LESSTHAN Then
                Return New Source.ElementNode(LocationFrom(FirstToken.Location), File, Value)
            End If

            'Get passed generic types
            Dim PassedGenericTypes As IEnumerable(Of Source.Type) = GetPassedGenericTypes()

            'Return
            Return New Source.GenericElementNode(LocationFrom(FirstToken.Location), File, Value, PassedGenericTypes)


        End If

        'Nothing found -> throw exception
        Throw New SyntaxException("A expression was expected here", Tokens.Current.Location)

    End Function

End Class
