Namespace AST
    Public MustInherit Class ExpressionNode
        Inherits AST.Node

        Public Sub New(Location As Location)
            MyBase.New(Location)
        End Sub

        Public MustOverride Function GetExpressionReturnType(Context As Context) As TypeSystem.Type
        Public MustOverride Function CompileExpression(Scope As Scope) As TypeSystem.Type

    End Class
End Namespace