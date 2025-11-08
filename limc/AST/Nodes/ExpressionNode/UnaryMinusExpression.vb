Namespace AST
    Public Class UnaryMinusExpression
        Inherits ExpressionNode

        Private Value As ExpressionNode

        Public Sub New(Value As ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.Value = Value
        End Sub

        Private Function GetRelation(Context As Context.Context) As Lazy.Relation
            Return Value.GetExpressionReturnType(Context).GetRelation(TypeSystem.RelationType.RELATION_UNARY_MINUS, {}, Location)
        End Function

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type
            Return GetRelation(Context).ReturnType
        End Function

        Public Overrides Function CompileExpression(Scope As Context.Scope) As String
            Return GetRelation(Scope).CompileCall(Value, {}, Scope, Location)
        End Function

    End Class
End Namespace