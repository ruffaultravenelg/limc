Namespace AST
    Public Class BracketsExpression
        Inherits ExpressionNode

        Private Target As ExpressionNode
        Private Arguments As IEnumerable(Of ExpressionNode)

        Public Sub New(Target As ExpressionNode, Arguments As IEnumerable(Of ExpressionNode), Location As Location)
            MyBase.New(Location)
            Me.Target = Target
            Me.Arguments = Arguments
        End Sub

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type
            Return Target.GetExpressionReturnType(Context).GetRelation(TypeSystem.RelationType.RELATION_BRACKETS, Arguments.Select(Function(arg) arg.GetExpressionReturnType(Context)), Location).ReturnType
        End Function

        Public Overrides Function CompileExpression(Scope As Context.Scope) As String
            Return Target.GetExpressionReturnType(Scope).GetRelation(TypeSystem.RelationType.RELATION_BRACKETS, Arguments.Select(Function(arg) arg.GetExpressionReturnType(Scope)), Location).CompileCall(Target, Arguments, Scope, Location)
        End Function

    End Class
End Namespace