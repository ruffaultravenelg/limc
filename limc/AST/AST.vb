Namespace AST
    Public Class AbstractSyntaxTree

        '======================
        '===== PROPERTIES =====
        '======================
        Public ReadOnly Property Functions As New List(Of FunctionConstructNode)

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

        '======================
        '===== STATEMENTS =====
        '======================
        Private ReadOnly StatementFunctions As IEnumerable(Of Func(Of Integer, StatementNode)) = {AddressOf GetSourceStatementNode}

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
                For Each ParsingFunction As Func(Of Integer, StatementNode) In StatementFunctions
                    Try
                        Body.Add(ParsingFunction(StatementIndentation))
                        Exit For
                    Catch ex As NotTheRightElementException
                        TokenIndex = LineStartIndex
                    End Try
                Next

            End While
            Return Body
        End Function

        Private Function GetSourceStatementNode(Indentation As Integer) As SourceStatementNode

            If Not CurrentToken.Type = TokenType.SYMBOL_DOLLAR Then
                Throw New NotTheRightElementException()
            End If
            PushPosition()
            Advance()

            CheckTokenType(TokenType.VAL_STRING)
            Dim Source As String = CurrentToken.Value
            Advance()

            Return New SourceStatementNode(Source, RetrievePosition())

        End Function

        '======================
        '===== CONSTRUCTS =====
        '======================
        Private Function GetFunctionConstructNode() As FunctionConstructNode

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
            Return New FunctionConstructNode(FunctionName, Arguments, ReturnType, Body, RetrievePosition())

        End Function

    End Class
End Namespace