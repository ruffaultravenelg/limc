Namespace AST
    Public MustInherit Class ExpressionNode
        Inherits AST.Node

        Public Sub New(Location As Location)
            MyBase.New(Location)
        End Sub

        Public MustOverride Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type
        Public MustOverride Function CompileExpression(Writer As CWriter, Scope As Context.Scope) As String

    End Class
End Namespace