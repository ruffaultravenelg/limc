Public Module NodeParser

    '
    ' Parse a list of token into differents nodes
    '
    Public Sub Parse(File As LimSource, Tokens As List(Of Token))
        If Tokens.Count > 1 Then
            Dim Parser As New Parser(Tokens, File)
        End If
    End Sub

    '
    ' Represent a instance of a parser
    '   (parser can create parser in themselve)
    '
    Private Class Parser

        '==============================
        '======== INFORMATIONS ========
        '==============================
        Private Tokens As List(Of Token)
        Private File As LimSource
        Private Position As Integer
        Private CurrentToken As Token
        Private SaveStack As New Stack(Of Integer)

        ' All the constructs functions
        Private ReadOnly GetConstructs As IEnumerable(Of Func(Of ConstructNode)) = { 'IMPORTANT: Add all ConstructNode parsing functions here so that they are taken into account.
            AddressOf GetException,
            AddressOf GetFunction,
            AddressOf GetClass
        }

        ' All the statements
        Private ReadOnly GetStatements As IEnumerable(Of Func(Of Integer, StatementNode)) = { 'IMPORTANT: Add all StatementNode parsing functions here so that they are taken into account.
            AddressOf GetBreakStatement,
            AddressOf GetContinueStatement,
            AddressOf GetDeclareVariableStatement
        }

        ' Class content
        Private ReadOnly GetClassContents As IEnumerable(Of Func(Of Node)) = { 'IMPORTANT: Add all parsing functions here so that they are taken into account.
            AddressOf GetDelcareVariableStatementForScope1,
            AddressOf GetGetter,
            AddressOf GetSetter,
            AddressOf GetMethod
        }

        '=========================
        '======== ADVANCE ========
        '=========================
        Private Sub Advance(Optional Expected As String = "expression")

            ' Increment position
            Position += 1

            ' Not enough token to continue
            If Not Position < Tokens.Count Then
                Throw New NotEnoughTokenException($"A {Expected} was expected after this token :", Tokens(Position - 1).Location)
            End If

            ' Get token
            CurrentToken = Tokens(Position)

        End Sub

        '===============================================
        '======== NOT SURE IF IT IS WHAT I WANT ========
        '===============================================
        Private Sub NotSureIfItIsWhatIWant()

            'Save the token index in case
            SaveStack.Push(Position)

        End Sub

        '=============================================
        '======== NOT WHAT I AM SEARCHING FOR ========
        '=============================================
        Private Sub NotWhatIAmSearchingFor()

            ' Go just before were it was saved
            Position = SaveStack.Pop() - 1

            'Set current token
            Advance()

        End Sub

        '====================================
        '======== JUST WHAT I WANTED ========
        '====================================
        Private Sub JustWhatIWanted()

            ' Remove the save from the stack, we don't need it anymore
            SaveStack.Pop()

        End Sub

        '===============================
        '======== LOCATION FROM ========
        '===============================
        Private Function LocationFrom(Start As Location) As Location
            Return Start & Tokens(Position - 1).Location
        End Function

        '================================
        '======== HAS MORE TOKEN ========
        '================================
        Private Function HasMoreToken() As Boolean
            Return Position < Tokens.Count - 1
        End Function

        '=============================
        '======== CONSTRUCTOR ========
        '=============================
        Public Sub New(Tokens As List(Of Token), File As LimSource)

            ' Get tokens
            Me.Tokens = Tokens

            ' Get file
            Me.File = File

            ' Init position
            Position = -1
            Advance()

            ' Get each construct
            While HasMoreToken() AndAlso Position < Tokens.Count - 2

                ' Get node
                Dim Construct As ConstructNode = GetConstruct()

                ' Sort
                If TypeOf Construct Is ExceptionConstructNode Then
                    File.Exceptions.Add(Construct)
                ElseIf TypeOf Construct Is FunctionConstructNode Then
                    File.Functions.Add(Construct)
                End If

            End While

        End Sub

        '============================
        '======== GET FACTOR ========
        '============================
        Private Function GetFactor() As ExpressionNode

            'Save start
            Dim Start As Location = CurrentToken.Location
            Dim Tok As Token = CurrentToken

            'Integer
            If CurrentToken.Type = TokenType.INT Then
                Advance()
                Return New IntegerNode(Tok)
            End If

            'Parenthesis
            If CurrentToken.Type = TokenType.SYNTAX_LEFT_PARENTHESIS Then
                Advance()
                Dim Expression As ExpressionNode = GetExpression()
                If Not CurrentToken.Type = TokenType.SYNTAX_RIGHT_PARENTHESIS Then
                    Throw New SyntaxErrorException("As the expression was finished, a closing parenthesis was expected.", LocationFrom(Start))
                End If
                Advance()
                Return Expression
            End If

            'No factor found
            Throw New SyntaxErrorException("An expression was expected here.", CurrentToken.Location)

        End Function

        '================================
        '======== GET EXPRESSION ========
        '================================
        Private Function GetExpression() As ExpressionNode
            Return GetFactor()
        End Function

        '===============================
        '======== GET TYPE NODE ========
        '===============================
        Private Function GetTypeNode() As TypeNode

            ' Save location
            Dim Start As Location = CurrentToken.Location

            ' Type name
            If Not CurrentToken.Type = TokenType.WORD Then
                Throw New LocalizedException("A type was expected here.", "The name of a type was expected here.", CurrentToken.Location)
            End If
            Dim Name As String = CurrentToken.Value
            Advance()

            ' Passed Generic Types
            Dim PassedGenericType As New List(Of TypeNode)

            ' No passed generic types
            If Not CurrentToken.Type = TokenType.OPERATOR_LESSTHAN Then
                Return New TypeNode(LocationFrom(Start), Name, PassedGenericType)
            End If
            Advance()
            If CurrentToken.Type = TokenType.OPERATOR_MORETHAN Then
                Return New TypeNode(LocationFrom(Start), Name, PassedGenericType)
            End If

            ' Get each type
            While True

                ' Get type
                Dim Type As TypeNode = GetTypeNode()

                ' Comma -> continue
                If CurrentToken.Type = TokenType.SYNTAX_COMMA Then
                    Advance()
                    Continue While
                End If

                ' '>' -> end
                If CurrentToken.Type = TokenType.OPERATOR_MORETHAN Then
                    Advance()
                    Exit While
                End If

                ' Error
                Throw New LocalizedException("Unexpected character", "Only the characters "","" and "">"" were expected here.", CurrentToken.Location)

            End While

            ' Return result
            Return New TypeNode(LocationFrom(Start), Name, PassedGenericType)

        End Function

        '=======================================
        '======== GET GENERIC TYPE NODE ========
        '=======================================
        Private Function GetGenericTypeNode() As GenericTypeNode

            ' Save location
            Dim Start As Location = CurrentToken.Location

            ' Name
            If Not CurrentToken.Type = TokenType.WORD Then
                Throw New LocalizedException("The name of a generic type was expected here.", "This will act as an alias for a type you don't necessarily know in advance.", CurrentToken.Location)
            End If
            Dim Name As String = CurrentToken.Value
            Advance()

            ' Type
            Dim Contract As TypeNode = Nothing
            If CurrentToken.Type = TokenType.SYNTAX_COLON Then
                Advance()
                Contract = GetTypeNode()
            End If

            ' Return
            Return New GenericTypeNode(LocationFrom(Start), Name, Contract)

        End Function

        '==================================
        '======== GET GENRIC TYPES ========
        '==================================
        Private Function GetGenericTypeNodes() As List(Of GenericTypeNode)

            'Create result
            Dim Result As New List(Of GenericTypeNode)

            'Check '<'
            If Not CurrentToken.Type = TokenType.OPERATOR_LESSTHAN Then
                Return Result
                'Throw New LocalizedException("The ""<"" character was expected.", "A list of generic types was expected here, starting with ""<"".", CurrentToken.Location)
            End If
            Advance()

            'Check for a end '>'
            If CurrentToken.Type = TokenType.OPERATOR_MORETHAN Then
                Advance()
                Return Result
            End If

            'Get each types
            While True

                ' Get generic type
                Dim GenericType As GenericTypeNode = GetGenericTypeNode()

                ' Comma -> continue to the next
                If CurrentToken.Type = TokenType.SYNTAX_COMMA Then
                    Advance()
                    Continue While
                End If

                ' '>' -> end
                If CurrentToken.Type = TokenType.OPERATOR_MORETHAN Then
                    Advance()
                    Exit While
                End If

                ' Error
                Throw New LocalizedException("Unexpected character", "Only the characters "","" and "">"" were expected here.", CurrentToken.Location)

            End While

            'Return result
            Return Result

        End Function

        '===================================
        '======== GET ARGUMENT NODE ========
        '===================================
        Private Function GetArgumentNode() As FunctionArgumentNode

            'Save location
            Dim Start As Location = CurrentToken.Location

            'Get argument name
            If Not CurrentToken.Type = TokenType.WORD Then
                Throw New LocalizedException("An argument name was expected here.", "The name of an argument was expected here.", CurrentToken.Location)
            End If
            Dim Name As String = CurrentToken.Value
            Advance()

            'Get argument type
            If Not CurrentToken.Type = TokenType.SYNTAX_COLON Then
                Throw New LocalizedException("The type of argument was expected here.", "The "":"" character was expected here, followed by a type.", CurrentToken.Location)
            End If
            Advance()
            Dim Type As TypeNode = GetTypeNode()

            'Return
            Return New FunctionArgumentNode(LocationFrom(Start), Name, Type)

        End Function

        '====================================
        '======== GET ARGUMENT NODES ========
        '====================================
        Private Function GetArgumentNodes() As List(Of FunctionArgumentNode)

            'Create result
            Dim Result As New List(Of FunctionArgumentNode)

            'Check '('
            If Not CurrentToken.Type = TokenType.SYNTAX_LEFT_PARENTHESIS Then
                Return Result
                'Throw New LocalizedException("The ""("" character was expected.", "A list of arguments was expected here, starting with ""("".", CurrentToken.Location)
            End If
            Advance()

            'Check for a end ')'
            If CurrentToken.Type = TokenType.SYNTAX_RIGHT_PARENTHESIS Then
                Advance()
                Return Result
            End If

            'Get each types
            While True

                ' Get generic type
                Dim GenericType As FunctionArgumentNode = GetArgumentNode()

                ' Comma -> continue to the next
                If CurrentToken.Type = TokenType.SYNTAX_COMMA Then
                    Advance()
                    Continue While
                End If

                ' '>' -> end
                If CurrentToken.Type = TokenType.SYNTAX_RIGHT_PARENTHESIS Then
                    Advance()
                    Exit While
                End If

                ' Error
                Throw New LocalizedException("Unexpected character", "Only the characters "","" and "")"" were expected here.", CurrentToken.Location)

            End While

            'Return result
            Return Result

        End Function

        '==============================
        '======== GET CONTINUE ========
        '==============================
        Private Function GetContinueStatement(StatementIndentationLevel As Integer) As ContinueStatementNode

            'No break keyword
            If Not CurrentToken.Type = TokenType.KEYWORD_CONTINUE Then
                Return Nothing
            End If
            Advance()

            ' Return
            Return New ContinueStatementNode(CurrentToken.Location)

        End Function

        '===========================
        '======== GET BREAK ========
        '===========================
        Private Function GetBreakStatement(StatementIndentationLevel As Integer) As BreakStatementNode

            'No break keyword
            If Not CurrentToken.Type = TokenType.KEYWORD_BREAK Then
                Return Nothing
            End If
            Advance()

            ' Return
            Return New BreakStatementNode(CurrentToken.Location)

        End Function

        '======================================
        '======== GET DECLARE VARIABLE ========
        '======================================
        Private Function GetDelcareVariableStatementForScope1() As DeclareVariableStatementNode
            Return GetDeclareVariableStatement(1)
        End Function
        Private Function GetDeclareVariableStatement(StatementIndentationLevel As Integer) As DeclareVariableStatementNode

            'Save location
            Dim Start As Location = CurrentToken.Location

            'No break keyword
            If Not CurrentToken.Type = TokenType.KEYWORD_LET Then
                Return Nothing
            End If
            Advance()

            'Get variable name
            Dim VariableName As String
            If Not CurrentToken.Type = TokenType.WORD Then
                Throw New SyntaxErrorException("A variable name was expected here", CurrentToken.Location)
            End If
            VariableName = CurrentToken.Value
            Advance()

            'Get variable type
            Dim VariableType As TypeNode = Nothing
            If CurrentToken.Type = TokenType.SYNTAX_COLON Then
                Advance()
                VariableType = GetTypeNode()
            End If

            'Get value
            Dim VariableValue As ExpressionNode = Nothing
            If CurrentToken.Type = TokenType.OPERATOR_EQUAL Then
                Advance()
                VariableValue = GetExpression()
            End If

            'No value & no type = error
            If VariableType Is Nothing AndAlso VariableValue Is Nothing Then
                Throw New SyntaxErrorException("To declare a variable, you must at least specify its type or a default value, or both.", LocationFrom(Start))
            End If

            ' Return
            Return New DeclareVariableStatementNode(LocationFrom(Start), VariableName, VariableType, VariableValue)

        End Function

        '===============================
        '======== GET STATEMENT ========
        '===============================
        Private Function GetStatement(StatementIndentationLevel As Integer) As StatementNode

            ' Create the result
            Dim Result As StatementNode = Nothing

            ' Try searching for all constructs
            For Each ParsingStatement In GetStatements

                ' Save state
                NotSureIfItIsWhatIWant()

                ' Try parsing
                Result = ParsingStatement(StatementIndentationLevel)

                ' If it worked, stop
                If Result IsNot Nothing Then
                    JustWhatIWanted()
                    Exit For
                Else
                    NotWhatIAmSearchingFor()
                End If

            Next

            ' Nothing found
            If Result Is Nothing Then
                Throw New LocalizedException("No statment could be found here.", "A ""Statement"" node was expected here.", CurrentToken.Location)
            End If

            ' Return result
            Return Result

        End Function

        '===========================
        '======== GET LOGIC ========
        '===========================
        Private Function GetLogic(LogicIndentation As Integer) As List(Of StatementNode)

            'Create result
            Dim Result As New List(Of StatementNode)

            'Get each statement
            While True

                'No new line
                If Not CurrentToken.Type = TokenType.SYNTAX_LINESTART Then
                    Throw New LocalizedException("A new line was expected here.", "The start of a statement was expected here.", CurrentToken.Location)
                End If

                'Get current line indentation
                Dim LineIndentation As Integer = Integer.Parse(CurrentToken.Value)

                'Not in this scope
                If LineIndentation < LogicIndentation Then
                    Exit While
                End If
                Advance()

                'Not the right indentation
                If Not LineIndentation = LogicIndentation Then
                    Throw New LocalizedException("Indentation too high", "This scope only requires " & LogicIndentation.ToString & " indentation (instead of " & LineIndentation.ToString() & ").", CurrentToken.Location)
                End If

                'Get statement
                Result.Add(GetStatement(LogicIndentation))

            End While

            'Return result
            Return Result

        End Function

        '============================
        '======== GET SETTER ========
        '============================
        Private Function GetSetter() As SetterConstructNode

            ' Save location
            Dim Start As Location = CurrentToken.Location

            ' Exception keyword
            If Not CurrentToken.Type = TokenType.KEYWORD_SET Then
                Return Nothing
            End If
            Advance()

            ' Get name
            If Not CurrentToken.Type = TokenType.WORD Then
                Throw New SyntaxErrorException("The name of a property was expected here", CurrentToken.Location)
            End If
            Dim Name As String = CurrentToken.Value
            Advance()

            ' Return type here
            If CurrentToken.Type = TokenType.SYNTAX_COLON Then
                Throw New SyntaxErrorException("A setter has no type as such. The type to be entered is defined by the argument variable. Use the following syntax: ""set " & Name & " to newValue:type"".", CurrentToken.Location)
            End If

            ' To keyword
            If Not CurrentToken.Type = TokenType.KEYWORD_TO Then
                Throw New SyntaxErrorException("The ""to"" keyword was expected. Use the following syntax: ""set " & Name & " to newValue:type"".", CurrentToken.Location)
            End If
            Advance()

            ' New value name
            Dim NewValue As FunctionArgumentNode = GetArgumentNode()

            ' Get logic
            Dim Logic As List(Of StatementNode) = GetLogic(2) '2 because it's a method.

            ' Return result
            Return New SetterConstructNode(LocationFrom(Start), Logic, Name, NewValue)

        End Function

        '============================
        '======== GET GETTER ========
        '============================
        Private Function GetGetter() As GetterConstructNode

            ' Save location
            Dim Start As Location = CurrentToken.Location

            ' Exception keyword
            If Not CurrentToken.Type = TokenType.KEYWORD_GET Then
                Return Nothing
            End If
            Advance()

            ' Get name
            If Not CurrentToken.Type = TokenType.WORD Then
                Throw New SyntaxErrorException("The name of a property was expected here", CurrentToken.Location)
            End If
            Dim Name As String = CurrentToken.Value
            Advance()

            ' Return type
            Dim ReturnType As TypeNode = Nothing
            If CurrentToken.Type = TokenType.SYNTAX_COLON Then
                Advance()
                ReturnType = GetTypeNode()
            End If

            ' Get logic
            Dim Logic As List(Of StatementNode) = GetLogic(2) '2 because it's a method.

            ' Return result
            Return New GetterConstructNode(LocationFrom(Start), Logic, Name, ReturnType)

        End Function

        '============================
        '======== GET METHOD ========
        '============================
        Private Function GetMethod() As MethodConstructNode

            ' Save location
            Dim Start As Location = CurrentToken.Location

            ' Exception keyword
            If Not CurrentToken.Type = TokenType.KEYWORD_FUNC Then
                Return Nothing
            End If
            Advance()

            ' Get name
            If Not CurrentToken.Type = TokenType.WORD Then
                Throw New SyntaxErrorException("The name of a method was expected", CurrentToken.Location)
            End If
            Dim Name As String = CurrentToken.Value
            Advance()

            ' There is generic types
            If CurrentToken.Type = TokenType.OPERATOR_LESSTHAN Then
                Throw New SyntaxErrorException("A class method cannot have generic types. Please use the generic types of the class itself or an independent function.", CurrentToken.Location)
            End If

            ' Get arguments
            Dim Arguments As List(Of FunctionArgumentNode) = GetArgumentNodes()

            ' Return type
            Dim ReturnType As TypeNode = Nothing
            If CurrentToken.Type = TokenType.SYNTAX_COLON Then
                Advance()
                ReturnType = GetTypeNode()
            End If

            ' Get logic
            Dim Logic As List(Of StatementNode) = GetLogic(2) '2 because it's a method.

            ' Return result
            Return New MethodConstructNode(LocationFrom(Start), Logic, Name, Arguments, ReturnType)

        End Function

        '===========================
        '======== GET CLASS ========
        '===========================
        Private Function GetClass() As ClassConstructNode

            ' Save location
            Dim Start As Location = CurrentToken.Location

            ' Primitive
            Dim Primitive As Boolean = False
            If CurrentToken.Type = TokenType.KEYWORD_PRIMITIVE Then
                Primitive = True
                Advance()
            End If

            ' Exception keyword
            If Not CurrentToken.Type = TokenType.KEYWORD_CLASS Then
                Return Nothing
            End If
            Advance()

            ' Get name
            If Not CurrentToken.Type = TokenType.WORD Then
                Throw New SyntaxErrorException("The name of a class was expected here.", CurrentToken.Location)
            End If
            Dim Name As String = CurrentToken.Value
            Advance()

            ' Get generic types
            Dim GenericTypes As List(Of GenericTypeNode) = GetGenericTypeNodes()

            ' Get content
            Dim Content As New List(Of Node)
            While True

                'No new line
                If Not CurrentToken.Type = TokenType.SYNTAX_LINESTART Then
                    Throw New SyntaxErrorException("A new line was expected here.", CurrentToken.Location)
                End If

                'Get current line indentation
                Dim LineIndentation As Integer = Integer.Parse(CurrentToken.Value)

                'Not in this scope
                If LineIndentation < 1 Then
                    Exit While
                End If
                Advance()

                'Not the right indentation
                If Not LineIndentation = 1 Then
                    Throw New LocalizedException("Indentation too high", "This scope only requires 1 indentation (instead of " & LineIndentation.ToString() & ").", CurrentToken.Location)
                End If

                'Get statement
                Dim Result As Node = Nothing

                ' Try searching for all constructs
                For Each ParsingStatement In GetClassContents

                    ' Save state
                    NotSureIfItIsWhatIWant()

                    ' Try parsing
                    Result = ParsingStatement()

                    ' If it worked, stop
                    If Result IsNot Nothing Then
                        JustWhatIWanted()
                        Exit For
                    Else
                        NotWhatIAmSearchingFor()
                    End If

                Next

                ' Nothing found
                If Result Is Nothing Then
                    Throw New SyntaxErrorException("No class statement could be found here.", CurrentToken.Location)
                End If

                ' Return result
                Content.Add(Result)

            End While

            ' Return result
            Return New ClassConstructNode(LocationFrom(Start), Primitive, Name, GenericTypes, Content)

        End Function

        '==============================
        '======== GET FUNCTION ========
        '==============================
        Private Function GetFunction() As FunctionConstructNode

            ' Save location
            Dim Start As Location = CurrentToken.Location

            ' Exception keyword
            If Not CurrentToken.Type = TokenType.KEYWORD_FUNC Then
                Return Nothing
            End If
            Advance()

            ' Get name
            If Not CurrentToken.Type = TokenType.WORD Then
                Throw New LocalizedException("The name of a function was expected", "The function name must follow the ""func"" keyword.", CurrentToken.Location)
            End If
            Dim Name As String = CurrentToken.Value
            Advance()

            ' Get generic types
            Dim GenericTypes As List(Of GenericTypeNode) = GetGenericTypeNodes()

            ' Get arguments
            Dim Arguments As List(Of FunctionArgumentNode) = GetArgumentNodes()

            ' Return type
            Dim ReturnType As TypeNode = Nothing
            If CurrentToken.Type = TokenType.SYNTAX_COLON Then
                Advance()
                ReturnType = GetTypeNode()
            End If

            ' Get logic
            Dim Logic As List(Of StatementNode) = GetLogic(1) '1 because it's just a function. The basic scope of a function is 1 indentation where a method is 2.

            ' Return result
            Return New FunctionConstructNode(LocationFrom(Start), Logic, Name, GenericTypes, Arguments, ReturnType)

        End Function

        '===============================
        '======== GET EXCEPTION ========
        '===============================
        Private Function GetException() As ExceptionConstructNode

            ' Save location
            Dim Start As Location = CurrentToken.Location

            ' Exception keyword
            If Not CurrentToken.Type = TokenType.KEYWORD_EXCEPTION Then
                Return Nothing
            End If
            Advance()

            ' Get name
            If Not CurrentToken.Type = TokenType.WORD Then
                Throw New LocalizedException("The name of an exception was expected", "The exception name must follow the ""exception"" keyword.", CurrentToken.Location)
            End If
            Dim Name As String = CurrentToken.Value
            Advance()

            ' Return result
            Return New ExceptionConstructNode(LocationFrom(Start), Name)

        End Function

        '===============================
        '======== GET CONSTRUCT ========
        '===============================
        Private Function GetConstruct() As ConstructNode

            'New line
            If Not CurrentToken.Type = TokenType.SYNTAX_LINESTART Then
                Throw New LocalizedException("A new line was expected here", "A new block of code was expected here. Check that the upper block is finished correctly.", CurrentToken.Location)
            End If

            'Indentation
            If Not CurrentToken.Value = "0" Then
                Throw New LocalizedException("No indentation was expected here.", "A new block of code was expected here.", CurrentToken.Location)
            End If

            ' Skip indentation
            Advance()

            ' Is the construct exported ?
            Dim Exported As Boolean = False
            If CurrentToken.Type = TokenType.KEYWORD_EXPORT Then
                Exported = True
                Advance()
            End If

            ' Create the result
            Dim Result As ConstructNode = Nothing

            ' Try searching for all constructs
            For Each ParsingFunction In GetConstructs

                ' Save state
                NotSureIfItIsWhatIWant()

                ' Try parsing
                Result = ParsingFunction()

                ' If it worked, stop
                If Result IsNot Nothing Then
                    JustWhatIWanted()
                    Exit For
                Else
                    NotWhatIAmSearchingFor()
                End If

            Next

            ' Nothing found
            If Result Is Nothing Then
                Throw New LocalizedException("No code blocks could be found here.", "A ""Construct"" node was expected here. For example, a function, a class, an exception or something else.", CurrentToken.Location)
            End If

            ' Change it's visibility (exported or not)
            Result.Exported = Exported

            ' Return result
            Return Result

        End Function

    End Class

End Module
