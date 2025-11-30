Namespace AST
    Public Class AbstractSyntaxTree

        '======================
        '===== PROPERTIES =====
        '======================
        Public ReadOnly Property Functions As New List(Of FunctionConstruct)
        Public ReadOnly Property Constants As New List(Of DeclareConstantWithValueConstruct)
        Public ReadOnly Property TypeConstructs As New List(Of IGenerateType)
        Public ReadOnly Property Include_Imports As New List(Of ImportNode)
        Public ReadOnly Property Include_Uses As New List(Of UseNode)

        '==================
        '===== TOKENS =====
        '==================
        Private ReadOnly Tokens As IEnumerable(Of Token)
        Private TokenIndex As Integer = 0

        Private ReadOnly Property CurrentToken As Token
            Get
                If TokenIndex < Tokens.Count Then
                    Return Tokens(TokenIndex)
                Else
                    Throw New EndOfFileError(Tokens(TokenIndex - 2).Location)
                End If
            End Get
        End Property

        Private Sub Advance(Optional Count As Integer = 1)
            TokenIndex += Count
        End Sub
        Private Sub Retreat(Optional Count As Integer = 1)
            TokenIndex -= Count
        End Sub

        Private Sub CheckTokenType(TokenType As TokenType, Optional Message As String = "")
            If Not CurrentToken.Type = TokenType Then
                Dim Location As Location
                If CurrentToken.Type = TokenType.LINESTART AndAlso TokenIndex - 1 >= 0 Then
                    Location = New Location(Tokens(TokenIndex - 1).Location)
                    Location.FromCol = Location.ToCol - 1
                Else
                    Location = CurrentToken.Location
                End If
                If Message = "" Then
                    Throw New UnexpectedTokenError(Location, TokenType)
                Else
                    Throw New UnexpectedTokenError(Location, Message)
                End If
            End If
        End Sub
        Private Sub IsTheRightToken(TokenType As TokenType)
            If Not CurrentToken.Type = TokenType Then
                Throw New NotTheRightElementException()
            End If
        End Sub
        Private Sub CheckNewScopeStart(WantedIndentation As Integer)
            CheckTokenType(TokenType.LINESTART)
            If Not CurrentToken.Value = WantedIndentation Then
                Throw New SyntaxError($"An indentation of {WantedIndentation} was expected here.", CurrentToken.Location)
            End If
        End Sub

        Private StartPositions As New Stack(Of Location)
        Private Sub PushPosition()
            StartPositions.Push(CurrentToken.Location)
        End Sub
        Private Function PopPosition() As Location
            Return StartPositions.Pop()
        End Function
        Private Function RetrievePosition() As Location
            Return StartPositions.Pop() + Tokens(TokenIndex - 1).Location
        End Function
        Private ReadOnly Property LastTokPos As Location
            Get
                Return Tokens(TokenIndex - 1).Location
            End Get
        End Property

        '=======================
        '===== CONSTRUCTOR =====
        '=======================
        Private ReadOnly FileConstructs As IEnumerable(Of Func(Of ConstructNode)) = {AddressOf GetFunctionConstructNode, AddressOf GetConstantConstructNode, AddressOf GetRecordConstructNode}

        Public Sub New(Tokens As IEnumerable(Of Token))

            If Tokens.Count < 2 Then
                Exit Sub
            End If
            Me.Tokens = Tokens

            HandleIncludes()

            While TokenIndex < Tokens.Count - 1

                'Check linestart
                CheckTokenType(TokenType.LINESTART)
                If Not CurrentToken.Value = 0 Then
                    Throw New IndentationError(CurrentToken.Location, 0)
                End If
                Advance()

                'Exported
                Dim Exported As Boolean = False
                If CurrentToken.Type = TokenType.KEYWORD_EXPORT Then
                    Exported = True
                    Advance()
                End If

                'Try parsing a function
                Dim Construct As ConstructNode = Nothing
                Dim LineStartIndex As Integer = TokenIndex
                For Each GetConstruct In FileConstructs
                    Try
                        Construct = GetConstruct()
                        Exit For
                    Catch ex As NotTheRightElementException
                        TokenIndex = LineStartIndex
                    End Try
                Next

                'We don't know what construct we have in front of us
                If Construct Is Nothing Then
                    Throw New UnexpectedTokenError(CurrentToken.Location, "A construct was expected there (function, structure, etc.).")
                End If

                'Set exported
                Construct.SetExported(Exported)

                'Explode to differents properties (weird i know but i mean it work well)
                If TypeOf Construct Is FunctionConstruct Then
                    Functions.Add(Construct)
                ElseIf TypeOf Construct Is DeclareConstantWithValueConstruct Then
                    Constants.Add(Construct)
                ElseIf TypeOf Construct Is IGenerateType Then
                    TypeConstructs.Add(Construct)
                Else
                    Throw New InternalError()
                End If

            End While

        End Sub

        '====================
        '===== INCLUDES =====
        '====================
        Private Sub HandleIncludes()
            While TokenIndex < Tokens.Count - 1

                ' Check current line
                CheckTokenType(TokenType.LINESTART)
                Advance()
                Dim StartLocation As Location = CurrentToken.Location

                'Check for import
                If CurrentToken.Type = TokenType.KEYWORD_IMPORT Then
                    Advance()

                    If CurrentToken.Type = TokenType.TEXT Then
                        Include_Imports.Add(New ImportLibNode(CurrentToken.Location, CurrentToken.Value))
                        Advance()
                        Continue While
                    ElseIf CurrentToken.Type = TokenType.VAL_STRING Then
                        Include_Imports.Add(New ImportPathNode(CurrentToken.Location, CurrentToken.Value))
                        Advance()
                        Continue While
                    End If

                    Throw New SyntaxError("An import must be followed by a path to a ""lim"" file or the name of a library.", CurrentToken.Location)

                End If

                'Check for use
                If CurrentToken.Type = TokenType.KEYWORD_USE Then
                    Advance()

                    If CurrentToken.Type = TokenType.TEXT Then
                        Include_Uses.Add(New UseLibNode(CurrentToken.Location, CurrentToken.Value))
                        Advance()
                        Continue While
                    ElseIf CurrentToken.Type = TokenType.VAL_STRING Then
                        Dim Filepath As String = CurrentToken.Value
                        Advance()
                        CheckTokenType(TokenType.KEYWORD_AS, $"Importing a file as a module requires adding a module name. The syntax is as follows:{Environment.NewLine}{vbTab}use ""other.lim"" as other.")
                        Advance()
                        CheckTokenType(TokenType.TEXT, $"Importing a file as a module requires adding a module name. The syntax is as follows:{Environment.NewLine}{vbTab}use ""other.lim"" as other.")
                        Include_Uses.Add(New UsePathNode(StartLocation + CurrentToken.Location, Filepath, CurrentToken.Value))
                        Advance()
                        Continue While
                    End If

                    Throw New SyntaxError("An import must be followed by a path to a ""lim"" file or the name of a library.", CurrentToken.Location)

                End If

                ' Check for $include
                If CurrentToken.Type = TokenType.KEYWORD_INCLUDE Then
                    Advance()

                    If CurrentToken.Type = TokenType.TEXT Then
                        Throw New SyntaxError("A C include header must be enclosed in quotation marks. Examples: $include ""<unistd.h>""", CurrentToken.Location)
                    End If
                    If Not CurrentToken.Type = TokenType.VAL_STRING Then
                        Throw New SyntaxError("An $include must be followed by C header.", CurrentToken.Location)
                    End If

                    CAPI.HandleInclude(CurrentToken.Value, StartLocation + CurrentToken.Location)
                    Advance()
                    Continue While

                End If

                ' Nothing found
                Retreat()
                Exit While

            End While
        End Sub

        '=========================
        '===== MISCELLANEOUS =====
        '=========================
        Private Function GetTypeNode() As TypeNode
            PushPosition()

            'Rack (static array) : 5<int>
            If CurrentToken.Type = TokenType.VAL_INT Then

                Dim ElementCounts As Integer = CurrentToken.Value
                Advance()
                CheckTokenType(TokenType.SYMBOL_LESSTHAN, "The type of element contained in the array must be specified. For example: 5<user>")
                Advance()
                Dim ElementType As TypeNode = GetTypeNode()
                CheckTokenType(TokenType.SYMBOL_GREATERTHAN, "A array refers to a single type, it should be a closing symbol "">"".")
                Advance()

                Return New AST.RackTypeNode(ElementCounts, ElementType, RetrievePosition())

            End If

            'Type name
            CheckTokenType(TokenType.TEXT)
            Dim TypeName As String = CurrentToken.Value
            Advance()

            'Module?
            Dim ModuleName As String = ""
            If CurrentToken.Type = TokenType.OP_MODULE_RESOLVER Then
                Advance()
                CheckTokenType(TokenType.TEXT)
                ModuleName = TypeName
                TypeName = CurrentToken.Value
                If TypeName = "fun" Then
                    Throw New SyntaxError("The name ""fun"" is reserved for the eponymous type. No module should be associated with it.", RetrievePosition())
                End If
                Advance()
            End If

            'Arguments <arg1,arg2,...>
            Dim GenericArguments As New List(Of TypeNode)

            If CurrentToken.Type = TokenType.SYMBOL_LESSTHAN Then
                Advance()
                If Not CurrentToken.Type = TokenType.SYMBOL_GREATERTHAN Then
                    GenericArguments.Add(GetTypeNode())
                    While CurrentToken.Type = TokenType.SYMBOL_COMMA
                        Advance()
                        GenericArguments.Add(GetTypeNode())
                    End While
                    CheckTokenType(TokenType.SYMBOL_GREATERTHAN, "A comma or "">"" was expected here.")
                End If
                Advance()
            End If

            'Function type : func<arg1,arg2><ret>
            If TypeName = "fun" Then

                'Return type
                Dim ReturnType As TypeNode = Nothing
                If CurrentToken.Type = TokenType.SYMBOL_LESSTHAN Then
                    Advance()
                    ReturnType = GetTypeNode()
                    CheckTokenType(TokenType.SYMBOL_GREATERTHAN, "A function can only have one return type, it should be a closing symbol "">"".")
                    Advance()
                End If

                Return New AST.FunctionTypeNode(GenericArguments, ReturnType, RetrievePosition())

            End If

            If ModuleName = "" Then
                Return New AST.SimpleTypeNode(TypeName, GenericArguments, RetrievePosition())
            Else
                Return New AST.ModuleTypeNode(ModuleName, TypeName, GenericArguments, RetrievePosition())
            End If

        End Function

        Private Function GetArgumentNode() As ArgumentNode

            PushPosition()
            CheckTokenType(TokenType.TEXT)
            Dim ArgumentName As String = CurrentToken.Value
            Advance()
            CheckTokenType(TokenType.SYMBOL_COLON)
            Advance()
            Dim ArgumentType As TypeNode = GetTypeNode()

            Return New AST.ArgumentNode(ArgumentName, ArgumentType, RetrievePosition())

        End Function

        Private Function GetPassedGenericTypes() As IEnumerable(Of TypeNode)
            Dim Result As New List(Of TypeNode)

            ' No values
            If Not CurrentToken.Type = TokenType.SYMBOL_LESSTHAN Then
                Return Result
            End If
            Advance()
            If CurrentToken.Type = TokenType.SYMBOL_GREATERTHAN Then
                Advance()
                Return Result
            End If

            ' Get values
            While True
                Result.Add(GetTypeNode())

                If CurrentToken.Type = TokenType.SYMBOL_COMMA Then
                    Advance()
                    Continue While

                ElseIf CurrentToken.Type = TokenType.SYMBOL_GREATERTHAN Then
                    Advance()
                    Exit While

                Else
                    Throw New SyntaxError("A comma or a "">"" was expected here", CurrentToken.Location)

                End If
            End While

            Return Result
        End Function

        Private Function GetGenericTypeNames() As IEnumerable(Of String)
            Dim GenericArguments As New List(Of String)
            If CurrentToken.Type = TokenType.SYMBOL_LESSTHAN Then
                Advance()
                If Not CurrentToken.Type = TokenType.SYMBOL_GREATERTHAN Then
                    While True
                        If Not CurrentToken.Type = TokenType.TEXT Then
                            Throw New SyntaxError("A generic type name was expected here", CurrentToken.Location)
                        End If
                        GenericArguments.Add(CurrentToken.Value)
                        Advance()
                        If CurrentToken.Type = TokenType.SYMBOL_COMMA Then
                            Advance()
                        ElseIf CurrentToken.Type = TokenType.SYMBOL_GREATERTHAN Then
                            Exit While
                        Else
                            Throw New SyntaxError("A comma or a "">"" was expected here", CurrentToken.Location)
                        End If
                    End While
                End If
                Advance()
            End If
            Return GenericArguments
        End Function

        Private Function ContinueRecordExpression(ModuleName As String, RecordName As String, RecordGenericTypes As IEnumerable(Of TypeNode), BaseLocation As Location) As ExpressionNode
            CheckTokenType(TokenType.SYMBOL_LEFT_BRACE)

            ' Convert info into a typenode
            Dim RecordType As TypeNode
            If ModuleName <> "" Then
                RecordType = New ModuleTypeNode(ModuleName, RecordName, RecordGenericTypes, BaseLocation + LastTokPos)
            Else
                RecordType = New SimpleTypeNode(RecordName, RecordGenericTypes, BaseLocation + LastTokPos)
            End If

            ' No values
            Advance()
            If CurrentToken.Type = TokenType.SYMBOL_RIGHT_BRACE Then
                Advance()
                Return New RecordExpression(RecordType, {}, BaseLocation + LastTokPos)
            End If

            ' Get values
            Dim Values As New List(Of ExpressionNode)
            Values.Add(GetExpression())
            While CurrentToken.Type = TokenType.SYMBOL_COMMA
                Advance()
                Values.Add(GetExpression())
            End While
            CheckTokenType(TokenType.SYMBOL_RIGHT_BRACE, "A comma or a closing parenthesis was expected here.")
            Advance()

            ' Create node
            Return New RecordExpression(RecordType, Values, BaseLocation + LastTokPos)

        End Function

        '=======================
        '===== EXPRESSIONS =====
        '=======================

        ' \\\\\\ {value} //////
        Private Function GetFactor() As ExpressionNode

            'One token expressions
            Dim Tok As Token = CurrentToken
            Advance()

            Select Case Tok.Type

                Case TokenType.VAL_INT
                    Return New IntExpression(Tok.Value, Tok.Location)

                Case TokenType.VAL_FLOAT
                    Return New FloatExpression(Tok.Value, Tok.Location)

                Case TokenType.VAL_STRING
                    Return New StringExpression(Tok.Value, Tok.Location)

                Case TokenType.TEXT

                    If CurrentToken.Type = TokenType.OP_MODULE_RESOLVER Then
                        'tok::

                        Advance()
                        CheckTokenType(TokenType.TEXT, "The name of an element must follow the ""::"" operator. For example, ""math::min"".")
                        Dim NameTok As Token = CurrentToken
                        Advance()
                        Dim PassedGenericTypes As IEnumerable(Of TypeNode) = GetPassedGenericTypes()
                        If CurrentToken.Type = TokenType.SYMBOL_LEFT_BRACE Then
                            Return ContinueRecordExpression(Tok.Value, NameTok.Value, PassedGenericTypes, Tok.Location)
                        Else
                            If PassedGenericTypes.Count > 0 Then
                                Return New ModuleResolverGenericElementExpression(Tok.Value, NameTok.Value, PassedGenericTypes, Tok.Location + Tokens(TokenIndex - 1).Location)
                            Else
                                Return New ModuleResolverExpression(Tok.Value, NameTok.Value, Tok.Location + NameTok.Location)
                            End If
                        End If


                    ElseIf CurrentToken.Type = TokenType.SYMBOL_LESSTHAN Then
                        'tok<

                        Dim PassedGenericTypes As IEnumerable(Of TypeNode) = GetPassedGenericTypes()
                        If CurrentToken.Type = TokenType.SYMBOL_LEFT_BRACE Then
                            Return ContinueRecordExpression("", Tok.Value, PassedGenericTypes, Tok.Location)
                        Else
                            If PassedGenericTypes.Count > 0 Then
                                Return New GenericElementExpression(Tok.Value, PassedGenericTypes, Tok.Location + Tokens(TokenIndex - 1).Location)
                            Else
                                Return New ElementExpression(Tok.Value, Tok.Location)
                            End If
                        End If

                    Else
                        'tok

                        If CurrentToken.Type = TokenType.SYMBOL_LEFT_BRACE Then
                            Return ContinueRecordExpression("", Tok.Value, {}, Tok.Location)
                        Else
                            Return New ElementExpression(Tok.Value, Tok.Location)
                        End If
                    End If

                Case TokenType.SYMBOL_LEFT_BRACE

                    ' Empty array
                    If CurrentToken.Type = TokenType.SYMBOL_RIGHT_BRACE Then
                        Throw New SyntaxError("A rack must contain at least one item in order to retrieve the rack type.", Tok.Location + CurrentToken.Location)
                    End If

                    ' Get values
                    Dim Elements As New List(Of ExpressionNode)
                    While True
                        Elements.Add(GetExpression())

                        If CurrentToken.Type = TokenType.SYMBOL_COMMA Then
                            Advance()
                            Continue While
                        ElseIf CurrentToken.Type = TokenType.SYMBOL_RIGHT_BRACE Then
                            Advance()
                            Exit While
                        Else
                            Throw New SyntaxError("A comma or a ""}"" was expected here.", CurrentToken.Location)
                        End If

                    End While

                    Return New RackExpression(Elements, Tok.Location + Tokens(TokenIndex - 1).Location)

                Case TokenType.SYMBOL_LEFT_PARENTHESIS
                    Dim Expression As ExpressionNode = GetExpression()
                    CheckTokenType(TokenType.SYMBOL_RIGHT_PARENTHESIS, "A closing parenthesis is expected here to end the expression.")
                    Advance()
                    Return Expression

                Case TokenType.SYMBOL_MINUS
                    Dim Value As ExpressionNode = GetFactor()
                    Return New UnaryMinusExpression(Value, Tok.Location + Value.Location)

                Case TokenType.VAL_BOOL
                    Return New BooleanExpression(Tok.Value, Tok.Location)

                Case TokenType.KEYWORD_NOT
                    Dim Expression As ExpressionNode = GetExpression()
                    Return New UnaryNotExpression(Expression, Tok.Location + Expression.Location)

            End Select

            'Error
            Throw New SyntaxError("A factor expression was expected here.", Tok.Location)

        End Function

        ' \\\\\\ {expression}(args) / {expression}[args} / {expression}.{child} //////
        Private Function GetCallBracketChild() As ExpressionNode

            Dim Expression As ExpressionNode = GetFactor()

            While True
                If CurrentToken.Type = TokenType.SYMBOL_LEFT_PARENTHESIS Then

                    Advance()
                    Dim Arguments As New List(Of ExpressionNode)
                    If Not CurrentToken.Type = TokenType.SYMBOL_RIGHT_PARENTHESIS Then
                        Arguments.Add(GetExpression())
                        While CurrentToken.Type = TokenType.SYMBOL_COMMA
                            Advance()
                            Arguments.Add(GetExpression())
                        End While
                        CheckTokenType(TokenType.SYMBOL_RIGHT_PARENTHESIS, "A comma or a closing parenthesis was expected here.")
                    End If
                    Advance()
                    Expression = New FunctionCallExpression(Expression, Arguments, Expression.Location + Tokens(TokenIndex - 1).Location)

                ElseIf CurrentToken.Type = TokenType.SYMBOL_POINT Then

                    Advance()
                    CheckTokenType(TokenType.TEXT, "A dot must be followed by the name of an element or the value of an enumeration.")
                    Expression = New AttributeExpression(Expression, CurrentToken.Value, Expression.Location + CurrentToken.Location)
                    Advance()

                ElseIf CurrentToken.Type = TokenType.SYMBOL_LEFT_BRACKETS Then

                    Advance()
                    Dim Arguments As New List(Of ExpressionNode)
                    If Not CurrentToken.Type = TokenType.SYMBOL_RIGHT_BRACKETS Then
                        Arguments.Add(GetExpression())
                        While CurrentToken.Type = TokenType.SYMBOL_COMMA
                            Advance()
                            Arguments.Add(GetExpression())
                        End While
                        CheckTokenType(TokenType.SYMBOL_RIGHT_BRACKETS, "A comma or a closing bracket was expected here.")
                    End If
                    Advance()
                    Expression = New BracketsExpression(Expression, Arguments, Expression.Location + Tokens(TokenIndex - 1).Location)

                Else
                    Exit While
                End If
            End While

            Return Expression

        End Function

        ' \\\\\\ {expression} */% {expression} //////
        Private Shared TokOpToRelOp_Divide As New Dictionary(Of TokenType, TypeSystem.RelationType) From {
            {TokenType.SYMBOL_MULTIPLICATE, TypeSystem.RelationType.RELATION_MULT},
            {TokenType.SYMBOL_DIVIDE, TypeSystem.RelationType.RELATION_DIV},
            {TokenType.SYMBOL_MODULO, TypeSystem.RelationType.RELATION_MODULO}
        }
        Private Function GetDivideOperation() As ExpressionNode

            Dim Left As ExpressionNode = GetCallBracketChild()

            While TokOpToRelOp_Divide.ContainsKey(CurrentToken.Type)

                Dim Op As TypeSystem.RelationType = TokOpToRelOp_Divide(CurrentToken.Type)
                Advance()
                Dim Right As ExpressionNode = GetCallBracketChild()

                Left = New NumericalOperationExpression(Left, Op, Right, Left.Location + Right.Location)

            End While

            Return Left

        End Function

        ' \\\\\\ {expression} +/- {expression} //////
        Private Shared TokOpToRelOp_Plus As New Dictionary(Of TokenType, TypeSystem.RelationType) From {
            {TokenType.SYMBOL_PLUS, TypeSystem.RelationType.RELATION_ADD},
            {TokenType.SYMBOL_MINUS, TypeSystem.RelationType.RELATION_SUB}
        }
        Private Function GetPlusOperation() As ExpressionNode

            Dim Left As ExpressionNode = GetDivideOperation()

            While TokOpToRelOp_Plus.ContainsKey(CurrentToken.Type)

                Dim Op As TypeSystem.RelationType = TokOpToRelOp_Plus(CurrentToken.Type)
                Advance()
                Dim Right As ExpressionNode = GetDivideOperation()

                Left = New NumericalOperationExpression(Left, Op, Right, Left.Location + Right.Location)

            End While

            Return Left

        End Function

        ' \\\\\\ {expression} =/> {expression} //////
        Private Shared TokOpToRelOp_Comp As New Dictionary(Of TokenType, TypeSystem.RelationType) From {
            {TokenType.SYMBOL_EQUAL, TypeSystem.RelationType.RELATION_EQUAL},
            {TokenType.SYMBOL_GREATERTHAN, TypeSystem.RelationType.RELATION_GREATERTHAN},
            {TokenType.SYMBOL_GREATERTHANEQUAL, TypeSystem.RelationType.RELATION_GREATERTHANEQUAL},
            {TokenType.SYMBOL_LESSTHAN, TypeSystem.RelationType.RELATION_LESSTHAN},
            {TokenType.SYMBOL_LESSTHANEQUAL, TypeSystem.RelationType.RELATION_LESSTHANEQUAL}
        }
        Private Function GetComparisonOperation() As ExpressionNode

            Dim Left As ExpressionNode = GetPlusOperation()

            While TokOpToRelOp_Comp.ContainsKey(CurrentToken.Type)

                Dim Op As TypeSystem.RelationType = TokOpToRelOp_Comp(CurrentToken.Type)
                Advance()
                Dim Right As ExpressionNode = GetPlusOperation()

                Left = New ComparisonOperationExpression(Left, Op, Right, Left.Location + Right.Location)

            End While

            Return Left

        End Function

        ' \\\\\\ {expression} AND/OR {expression} //////
        Private Shared Bool_Ops As IEnumerable(Of TokenType) = {TokenType.KEYWORD_AND, TokenType.KEYWORD_OR}
        Private Function GetBooleanOperation() As ExpressionNode

            Dim Left As ExpressionNode = GetComparisonOperation()

            While Bool_Ops.Contains(CurrentToken.Type)

                Dim Op As TypeSystem.RelationType = CurrentToken.Type
                Advance()
                Dim Right As ExpressionNode = GetPlusOperation()

                Left = New BooleanOperationExpression(Left, Op, Right, Left.Location + Right.Location)

            End While

            Return Left

        End Function

        Private Function GetExpression() As ExpressionNode
            Return GetBooleanOperation()
        End Function

        '======================
        '===== STATEMENTS =====
        '======================
        Private ReadOnly StatementFunctions As IEnumerable(Of Func(Of Integer, StatementNode)) = {AddressOf GetSourceStatementNode, AddressOf GetVariableDeclaration, AddressOf GetConstantDeclaration, AddressOf GetPanicStatement, AddressOf GetIfStatement, AddressOf GetForStatement, AddressOf GetReturnStatement, AddressOf GetWhileStatement, AddressOf GetBreakStatement, AddressOf GetContinueStatement, AddressOf GetProcedureCallStatement, AddressOf GetAssignStatement} 'Assign should be at the end

        Private Function GetBody(StatementIndentation As Integer) As IEnumerable(Of StatementNode)
            Dim Body As New List(Of StatementNode)
            While True

                'Check line & indentation
                CheckTokenType(TokenType.LINESTART)
                If CurrentToken.Value < StatementIndentation Then
                    Exit While
                End If
                If CurrentToken.Value > StatementIndentation Then
                    Throw New IndentationError(CurrentToken.Location, StatementIndentation)
                End If
                Advance()

                'Search statement
                Dim LineStartIndex As Integer = TokenIndex
                Dim StatementToAppend As StatementNode = Nothing
                For Each ParsingFunction As Func(Of Integer, StatementNode) In StatementFunctions
                    Try
                        StatementToAppend = ParsingFunction(StatementIndentation)
                        Exit For
                    Catch ex As NotTheRightElementException
                        TokenIndex = LineStartIndex
                    End Try
                Next

                'Error
                If StatementToAppend IsNot Nothing Then
                    Body.Add(StatementToAppend)
                Else
                    Throw New SyntaxError("A statement was expected here. This is not a statement.", CurrentToken.Location)
                End If

            End While
            Return Body
        End Function

        Private Function GetSourceStatementNode(Indentation As Integer) As SourceStatement

            If Not CurrentToken.Type = TokenType.SOURCE_LINE Then
                Throw New NotTheRightElementException()
            End If

            Dim Source As String = CurrentToken.Value
            Advance()
            Dim Node As New SourceStatement(Source, Tokens(TokenIndex - 1).Location + CurrentToken.Location)
            Return Node

        End Function

        Private Function GetVariableDeclaration() As StatementNode

            If Not CurrentToken.Type = TokenType.KEYWORD_LET Then
                Throw New NotTheRightElementException()
            End If
            PushPosition()
            Advance()

            'Get name
            CheckTokenType(TokenType.TEXT, "A variable name was expected here.")
            Dim VariableName As String = CurrentToken.Value
            Advance()

            'Get type
            Dim VariableType As TypeNode = Nothing
            If CurrentToken.Type = TokenType.SYMBOL_COLON Then
                Advance()
                VariableType = GetTypeNode()
            End If

            'Get value
            Dim VariableValue As ExpressionNode = Nothing
            If CurrentToken.Type = TokenType.SYMBOL_EQUAL Then
                Advance()
                VariableValue = GetExpression()
            End If

            'Create node
            If VariableType IsNot Nothing AndAlso VariableValue Is Nothing Then
                Return New DeclareVariableWithTypeStatement(VariableName, VariableType, RetrievePosition())
            ElseIf VariableType Is Nothing AndAlso VariableValue IsNot Nothing Then
                'Value
                Return New DeclareVariableWithValueStatement(VariableName, VariableValue, RetrievePosition())
            ElseIf VariableType IsNot Nothing AndAlso VariableValue IsNot Nothing Then
                'Type
                Return New DeclareVariableWithTypeValueStatement(VariableName, VariableType, VariableValue, RetrievePosition())
            Else
                Throw New SyntaxError("A variable declaration must contains at least the type of the variable or a default value.", RetrievePosition())
            End If

        End Function

        Private Function GetConstantDeclaration() As StatementNode

            If Not CurrentToken.Type = TokenType.KEYWORD_CONST Then
                Throw New NotTheRightElementException()
            End If
            PushPosition()
            Advance()

            'Get name
            CheckTokenType(TokenType.TEXT, "A constant name was expected here.")
            Dim ConstantName As String = CurrentToken.Value
            Advance()

            'Get type
            Dim ConstantType As TypeNode = Nothing
            If CurrentToken.Type = TokenType.SYMBOL_COLON Then
                Advance()
                ConstantType = GetTypeNode()
            End If

            'Get value
            Dim ConstantValue As ExpressionNode = Nothing
            If CurrentToken.Type = TokenType.SYMBOL_EQUAL Then
                Advance()
                ConstantValue = GetExpression()
            End If

            'Create node
            If ConstantType IsNot Nothing AndAlso ConstantValue Is Nothing Then
                Return New DeclareConstantWithTypeStatement(ConstantName, ConstantType, RetrievePosition())
            ElseIf ConstantType Is Nothing AndAlso ConstantValue IsNot Nothing Then
                'Value
                Return New DeclareConstantWithValueStatement(ConstantName, ConstantValue, RetrievePosition())
            ElseIf ConstantType IsNot Nothing AndAlso ConstantValue IsNot Nothing Then
                'Type
                Return New DeclareVariableWithTypeValueStatement(ConstantName, ConstantType, ConstantValue, RetrievePosition())
            Else
                Throw New SyntaxError("A variable declaration must contains at least the type of the variable or a default value.", RetrievePosition())
            End If

        End Function

        Private Function GetAssignStatement() As StatementNode

            Dim TargetVariable As ExpressionNode = GetCallBracketChild()
            IsTheRightToken(TokenType.SYMBOL_EQUAL)
            Advance()

            Dim NewValue As ExpressionNode = GetExpression()

            If TypeOf TargetVariable Is AST.IAssignable Then
                Return New VariableAssignationStatement(TargetVariable, NewValue, TargetVariable.Location + NewValue.Location)
            Else
                Throw New SyntaxError("This expression is not a variable. No assignment possible.", TargetVariable.Location)
            End If

        End Function

        Private Function GetProcedureCallStatement() As StatementNode

            Dim Expression As ExpressionNode = GetExpression()

            If TypeOf Expression Is FunctionCallExpression Then
                Return New ProcedureCallStatement(Expression)
            Else
                Throw New NotTheRightElementException()
            End If

        End Function

        Private Function GetPanicStatement() As StatementNode

            If Not CurrentToken.Type = TokenType.KEYWORD_PANIC Then
                Throw New NotTheRightElementException()
            End If
            PushPosition()
            Advance()

            Dim Message As ExpressionNode = GetExpression()

            Return New PanicStatement(Message, RetrievePosition())

        End Function

        Private Function GetWhileStatement(ActualIndentation As Integer) As StatementNode

            If Not CurrentToken.Type = TokenType.KEYWORD_WHILE Then
                Throw New NotTheRightElementException()
            End If
            PushPosition()
            Advance()

            Dim Condition As ExpressionNode = GetExpression()
            CheckNewScopeStart(ActualIndentation + 1)

            Dim Body As IEnumerable(Of StatementNode) = GetBody(ActualIndentation + 1)

            Return New WhileStatement(Condition, Body, RetrievePosition())

        End Function

        Private Function GetBreakStatement() As StatementNode
            If CurrentToken.Type = TokenType.KEYWORD_BREAK Then
                Advance()
                Return New BreakStatement(LastTokPos)
            Else
                Throw New NotTheRightElementException()
            End If
        End Function

        Private Function GetContinueStatement() As StatementNode
            If CurrentToken.Type = TokenType.KEYWORD_CONTINUE Then
                Advance()
                Return New ContinueStatement(LastTokPos)
            Else
                Throw New NotTheRightElementException()
            End If
        End Function

        Private Function GetIfStatement(ActualIndentation As Integer) As StatementNode

            ' Check if statement
            If Not CurrentToken.Type = TokenType.KEYWORD_IF Then
                Throw New NotTheRightElementException
            End If
            PushPosition()
            Advance()

            ' Get main condition
            Dim MainCondition As ExpressionNode = GetExpression()

            ' Get main instructions
            CheckNewScopeStart(ActualIndentation + 1)
            Dim MainInstructions As IEnumerable(Of StatementNode) = GetBody(ActualIndentation + 1)

            ' Get elseif
            Dim ElseIfBlocks As New List(Of IfStatement.ElseifBlock)
            While CurrentToken.Value = ActualIndentation
                Advance()

                ' Exit if not "elseif"
                If Not CurrentToken.Type = TokenType.KEYWORD_ELSEIF Then
                    Retreat()
                    Exit While
                End If
                Advance()

                ' Get condition
                Dim ElseIfCondition As ExpressionNode = GetExpression()

                ' Get elseif instructions
                CheckNewScopeStart(ActualIndentation + 1)
                ElseIfBlocks.Add(New IfStatement.ElseifBlock(ElseIfCondition, GetBody(ActualIndentation + 1)))

            End While

            ' Check else
            Dim ElseInstructions As IEnumerable(Of StatementNode) = Nothing
            If CurrentToken.Value = ActualIndentation Then
                Advance()
                If CurrentToken.Type = TokenType.KEYWORD_ELSE Then
                    Advance()
                    CheckNewScopeStart(ActualIndentation + 1)
                    ElseInstructions = GetBody(ActualIndentation + 1)
                Else
                    Retreat()
                End If
            End If

            ' Return new if statement
            Return New IfStatement(MainCondition, MainInstructions, ElseIfBlocks, ElseInstructions, RetrievePosition())

        End Function

        Private Function GetReturnStatement() As StatementNode

            ' Check return statement
            If Not CurrentToken.Type = TokenType.KEYWORD_RETURN Then
                Throw New NotTheRightElementException()
            End If
            PushPosition()
            Advance()

            ' Get value
            Dim Value As ExpressionNode = GetExpression()

            ' Create & return node
            Return New ReturnStatement(Value, RetrievePosition())

        End Function

        Private Function GetForStatement(ActualIndentation As Integer) As StatementNode

            ' Check for loop
            If Not CurrentToken.Type = TokenType.KEYWORD_FOR Then
                Throw New NotTheRightElementException()
            End If
            PushPosition()
            Advance()

            ' Get variable name
            CheckTokenType(TokenType.TEXT, "A variable name was expected here")
            Dim VariableName As String = CurrentToken.Value
            Advance()

            ' Get variable type (optionnal)
            Dim VariableType As TypeNode = Nothing
            If CurrentToken.Type = TokenType.SYMBOL_COLON Then
                Advance()
                VariableType = GetTypeNode()
            End If

            ' for each VS simple for
            If CurrentToken.Type = TokenType.KEYWORD_IN Then
                Advance()
                Dim Sequence As ExpressionNode = GetExpression()
                CheckNewScopeStart(ActualIndentation + 1)
                Return New ForEachStatement(VariableName, VariableType, Sequence, GetBody(ActualIndentation + 1), RetrievePosition())

            Else
                Dim Value_From As ExpressionNode = Nothing
                If CurrentToken.Type = TokenType.KEYWORD_FROM Then
                    Advance()
                    Value_From = GetExpression()
                End If

                CheckTokenType(TokenType.KEYWORD_TO)
                Advance()
                Dim Value_To As ExpressionNode = GetExpression()

                CheckNewScopeStart(ActualIndentation + 1)
                Return New ForStatement(VariableName, VariableType, Value_From, Value_To, GetBody(ActualIndentation + 1), RetrievePosition())
            End If

        End Function

        '======================
        '===== CONSTRUCTS =====
        '======================
        Private Function GetFunctionConstructNode() As FunctionConstruct

            If Not CurrentToken.Type = TokenType.KEYWORD_FUNC Then
                Throw New NotTheRightElementException()
            End If

            'Get name
            PushPosition()
            Advance()
            CheckTokenType(TokenType.TEXT, "A function must have a name.")
            Dim FunctionName As String = CurrentToken.Value
            Advance()

            'Generic arguments
            Dim GenericArguments As IEnumerable(Of String) = GetGenericTypeNames()

            'Arguments
            Dim Arguments As New List(Of ArgumentNode)
            If CurrentToken.Type = TokenType.SYMBOL_LEFT_PARENTHESIS Then
                Advance()
                If Not CurrentToken.Type = TokenType.SYMBOL_RIGHT_PARENTHESIS Then
                    Arguments.Add(GetArgumentNode())
                    While CurrentToken.Type = TokenType.SYMBOL_COMMA
                        Advance()
                        Arguments.Add(GetArgumentNode())
                    End While
                    CheckTokenType(TokenType.SYMBOL_RIGHT_PARENTHESIS, "A comma or a closing parenthesis was expected here.")
                End If
                Advance()
            End If

            'Return type
            Dim ReturnType As TypeNode = Nothing
            If CurrentToken.Type = TokenType.SYMBOL_COLON Then
                Advance()
                ReturnType = GetTypeNode()
            End If

            'Body
            Dim Body As IEnumerable(Of StatementNode) = GetBody(1)

            'Return node
            Return New FunctionConstruct(FunctionName, GenericArguments, Arguments, ReturnType, Body, RetrievePosition())

        End Function

        Private Function GetConstantConstructNode() As DeclareConstantWithValueConstruct

            If Not CurrentToken.Type = TokenType.KEYWORD_CONST Then
                Throw New NotTheRightElementException()
            End If
            PushPosition()
            Advance()

            'Get name
            CheckTokenType(TokenType.TEXT, "A constant name was expected here.")
            Dim ConstantName As String = CurrentToken.Value
            Advance()

            'Get type
            If CurrentToken.Type = TokenType.SYMBOL_COLON Then
                Throw New SyntaxError("A file constant does not take an explicit type.", CurrentToken.Location)
            End If

            'Get value
            If Not CurrentToken.Type = TokenType.SYMBOL_EQUAL Then
                Throw New SyntaxError("A file constant must be assigned a value when declared.", CurrentToken.Location)
            End If
            Advance()
            Dim ConstantValue As ExpressionNode = GetExpression()

            'Create node
            Return New DeclareConstantWithValueConstruct(ConstantName, ConstantValue, RetrievePosition())

        End Function

        Private Function GetRecordConstructNode() As RecordConstruct

            ' Check if this is a record
            If Not CurrentToken.Type = TokenType.KEYWORD_RECORD Then
                Throw New NotTheRightElementException()
            End If

            ' Get name
            PushPosition()
            Advance()
            CheckTokenType(TokenType.TEXT, "A record must have a name")
            Dim Name As String = CurrentToken.Value
            Advance()

            ' Get generic types
            Dim GenericArguments As IEnumerable(Of String) = GetGenericTypeNames()

            'Check fields opening (
            CheckTokenType(TokenType.SYMBOL_LEFT_PARENTHESIS, "A record must have at least one field")
            Advance()
            If CurrentToken.Type = TokenType.SYMBOL_RIGHT_PARENTHESIS Then
                Throw New SyntaxError("A record must have at least one field", Tokens(TokenIndex - 1).Location + CurrentToken.Location) 'Location = two last tokens => ()
            End If

            'Get fields
            Dim Fields As New List(Of RecordConstruct.Field)
            While True
                PushPosition()

                'Get field name
                CheckTokenType(TokenType.TEXT, "A field name was expected there")
                Dim FieldName As String = CurrentToken.Value
                Advance()

                'Get field type
                CheckTokenType(TokenType.SYMBOL_COLON)
                Advance()
                Dim FieldType As TypeNode = GetTypeNode()

                ' Create field
                Fields.Add(New RecordConstruct.Field(FieldName, FieldType, RetrievePosition()))

                'End there ?
                If CurrentToken.Type = TokenType.SYMBOL_RIGHT_PARENTHESIS Then
                    Advance()
                    Exit While
                End If

                ' Check for comma
                CheckTokenType(TokenType.SYMBOL_COMMA, "A ')' or a ',' was expected here. Close the record or add a new field.")
                Advance()

            End While

            ' Create record
            Return New RecordConstruct(Name, GenericArguments, Fields, RetrievePosition())

        End Function

    End Class
End Namespace