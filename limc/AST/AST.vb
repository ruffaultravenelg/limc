Namespace AST
    Public Class AbstractSyntaxTree

        '======================
        '===== PROPERTIES =====
        '======================
        Public ReadOnly Property Functions As New List(Of FunctionConstruct)

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
                If Message = "" Then
                    Throw New UnexpectedTokenError(CurrentToken.Location, TokenType)
                Else
                    Throw New UnexpectedTokenError(CurrentToken.Location, Message)
                End If
            End If
        End Sub
        Private Sub IsTheRightToken(TokenType As TokenType)
            If Not CurrentToken.Type = TokenType Then
                Throw New NotTheRightElementException()
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

        '=======================
        '===== CONSTRUCTOR =====
        '=======================
        Public Sub New(Tokens As IEnumerable(Of Token))

            If Tokens.Count < 2 Then
                Exit Sub
            End If
            Me.Tokens = Tokens

            While TokenIndex < Tokens.Count - 1

                'Check linestart
                CheckTokenType(TokenType.LINESTART)
                If Not CurrentToken.Value = 0 Then
                    Throw New IndentationError(CurrentToken.Location, 0)
                End If
                Advance()
                Dim LineStartIndex As Integer = TokenIndex

                'Try parsing a function
                Try
                    Functions.Add(GetFunctionConstructNode())
                    Continue While
                Catch ex As NotTheRightElementException
                    TokenIndex = LineStartIndex
                End Try

                'If we arrive there, we don't know what construct we have in front of us
                Throw New UnexpectedTokenError(CurrentToken.Location, "A construct was expected there (function, structure, etc.).")

            End While

        End Sub

        '=========================
        '===== MISCELLANEOUS =====
        '=========================
        Private Function GetTypeNode() As TypeNode
            PushPosition()

            'Array : 5<int>
            If CurrentToken.Type = TokenType.VAL_INT Then

                Dim ArrayCount As Integer = CurrentToken.Value
                Advance()
                CheckTokenType(TokenType.SYMBOL_LESSTHAN, "The type of element contained in the array must be specified. For example: 5<user>")
                Advance()
                Dim ElementType As TypeNode = GetTypeNode()
                CheckTokenType(TokenType.SYMBOL_GREATERTHAN, "A array refers to a single type, it should be a closing symbol "">"".")
                Advance()

                Return New AST.ArrayTypeNode(ArrayCount, ElementType, RetrievePosition())

            End If

            'Type name
            CheckTokenType(TokenType.TEXT)
            Dim TypeName As String = CurrentToken.Value
            Advance()

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

            Return New AST.SimpleTypeNode(TypeName, GenericArguments, RetrievePosition())

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

        '=======================
        '===== EXPRESSIONS =====
        '=======================
        Private Function GetFactor() As ExpressionNode

            'One token expressions
            Dim Tok As Token = CurrentToken
            Advance()

            Select Case Tok.Type
                Case TokenType.VAL_INT
                    Return New IntExpression(Tok.Value, Tok.Location)
                Case TokenType.TEXT
                    Return New ElementExpression(Tok.Value, Tok.Location)
            End Select

            'Error
            Throw New SyntaxError("A factor expression was expected here.", Tok.Location)

        End Function
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
                    Expression = New FunctionCallExpression(Expression, Arguments, RetrievePosition())

                Else
                    Exit While
                End If
            End While

            Return Expression

        End Function
        Private Function GetExpression() As ExpressionNode
            Return GetCallBracketChild()
        End Function

        '======================
        '===== STATEMENTS =====
        '======================
        Private ReadOnly StatementFunctions As IEnumerable(Of Func(Of Integer, StatementNode)) = {AddressOf GetSourceStatementNode, AddressOf GetVariableDeclaration, AddressOf GetAssignStatement} 'Assign should be at the end

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

            Dim Node As New SourceStatement(CurrentToken.Value, CurrentToken.Location)
            Advance()
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

        Private Function GetAssignStatement() As StatementNode

            Dim TargetVariable As ExpressionNode = GetExpression()
            IsTheRightToken(TokenType.SYMBOL_EQUAL)
            Advance()

            Dim NewValue As ExpressionNode = GetExpression()

            If TypeOf TargetVariable Is AST.IAssignable Then
                Return New VariableAssignationStatement(TargetVariable, NewValue, TargetVariable.Location + NewValue.Location)
            Else
                Throw New SyntaxError("This expression is not a variable. No assignment possible.", TargetVariable.Location)
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
            Return New FunctionConstruct(FunctionName, Arguments, ReturnType, Body, RetrievePosition())

        End Function

    End Class
End Namespace