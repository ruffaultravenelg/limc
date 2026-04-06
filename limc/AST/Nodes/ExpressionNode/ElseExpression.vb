Namespace AST

    '
    ' call() else expr
    ' If call() raise an error, catch it and return expr
    '

    Public Class ElseExpression
        Inherits ExpressionNode

        Private CallExpression As FunctionCallExpression
        Private FallBackExpression As ExpressionNode

        Public Sub New(CallExpression As FunctionCallExpression, FallBackExpression As ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.CallExpression = CallExpression
            Me.FallBackExpression = FallBackExpression
        End Sub

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type

            ' Check for type error
            Dim FunctionReturnType As TypeSystem.Type = CallExpression.GetExpressionReturnType(Context)
            If FunctionReturnType Is Nothing Then
                Throw New TypeError("This function doesn't return any value", CallExpression.Location)
            End If

            Dim FallbackType As TypeSystem.Type = FallBackExpression.GetExpressionReturnType(Context)
            If FallbackType <> FallbackType Then
                Throw New TypeMismatchError(FunctionReturnType, FallbackType, FallBackExpression.Location)
            End If

            ' Return function's return type
            Return FunctionReturnType

        End Function

        Public Overrides Function CompileExpression(Writer As CWriter, Scope As Context.Scope) As String

            ' Check for type error
            Dim FunctionReturnType As TypeSystem.Type = CallExpression.GetExpressionReturnType(Scope)
            If FunctionReturnType Is Nothing Then
                Throw New TypeError("This function doesn't return any value", CallExpression.Location)
            End If

            Dim FallbackType As TypeSystem.Type = FallBackExpression.GetExpressionReturnType(Scope)
            If FallbackType <> FallbackType Then
                Throw New TypeMismatchError(FunctionReturnType, FallbackType, FallBackExpression.Location)
            End If

            ' Setup context to accept problem
            Writer.WriteLine($"{RUNTIME_CONTEXT_VARIABLE_NAME}->accept_problem = true;")
            Writer.WriteLine($"{RUNTIME_CONTEXT_VARIABLE_NAME}->problem = NULL;")

            ' Create temps variable to hold result
            Dim TempVar As String = CodeGen.Namer.Temp()
            Writer.WriteLine($"{FunctionReturnType.cRepresentation} {TempVar} = {CallExpression.CompileExpression(Writer, Scope)};")

            ' Check if there is a problem
            Writer.WriteLine($"if ({RUNTIME_CONTEXT_VARIABLE_NAME}->problem) {TempVar} = {FallBackExpression.CompileExpression(Writer, Scope)};")

            ' Reset accept_problem
            Writer.WriteLine($"{RUNTIME_CONTEXT_VARIABLE_NAME}->accept_problem = false;")

            ' Return value
            Return TempVar

        End Function

    End Class
End Namespace