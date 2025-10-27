Namespace AST
    Public Class ProcedureCallStatement
        Inherits StatementNode

        Private CallExpression As FunctionCallExpression

        Public Sub New(CallExpression As FunctionCallExpression)
            MyBase.New(CallExpression.Location)
            Me.CallExpression = CallExpression
        End Sub

        Public Overrides Sub Compile(Scope As Context.Scope)
            Scope.WriteLine(CallExpression.CompileExpression(Scope) & ";")
        End Sub

    End Class

End Namespace