Namespace AST
    Public Class ProcedureCallStatement
        Inherits StatementNode

        Private CallExpression As FunctionCallExpression

        Public Sub New(CallExpression As FunctionCallExpression)
            MyBase.New(CallExpression.Location)
            Me.CallExpression = CallExpression
        End Sub

        Public Overrides Sub Compile(Writer As CWriter, Scope As Context.Scope)
            Writer.WriteLine(CallExpression.CompileExpression(Writer, Scope) & ";")
        End Sub

    End Class

End Namespace