Namespace AST
    Public Class PanicStatement
        Inherits StatementNode

        Private Message As ExpressionNode

        Public Sub New(Message As AST.ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.Message = Message
        End Sub

        Public Overrides Sub Compile(Scope As Context.Scope)
            If Not Message.GetExpressionReturnType(Scope) = TypeSystem.Type.Str Then
                Throw New TypeMismatchError(TypeSystem.Type.Str, Message.GetExpressionReturnType(Scope), Location)
            End If
            Scope.WriteLine($"{CodeGen.PANIC_FUNCTION_NAME}({CodeGen.RuntimeContextVariableName}, {Message.CompileExpression(Scope)});")
        End Sub

    End Class

End Namespace