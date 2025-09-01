Namespace AST
    Public Class AbstractSyntaxTree

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
                GenericArguments.Add(GetTypeNode())
                While CurrentToken.Type = TokenType.SYMBOL_COMMA
                    Advance()
                    GenericArguments.Add(GetTypeNode())
                End While
                CheckTokenType(TokenType.SYMBOL_GREATERTHAN, "A comma or "">"" was expected here.")
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

        '=======================
        '===== EXPRESSIONS =====
        '=======================

        '======================
        '===== STATEMENTS =====
        '======================

        '======================
        '===== CONSTRUCTS =====
        '======================

    End Class
End Namespace