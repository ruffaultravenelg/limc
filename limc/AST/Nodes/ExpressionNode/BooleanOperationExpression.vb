Namespace AST
    Public Class BooleanOperationExpression
        Inherits ExpressionNode

        Private Left As ExpressionNode
        Private Op As TypeSystem.RelationType
        Private Right As ExpressionNode

        Public Sub New(Left As ExpressionNode, Op As TypeSystem.RelationType, Right As ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.Left = Left
            Me.Op = Op
            Me.Right = Right
        End Sub

        Private Function GetRelation(Context As Context.Context) As Lazy.Relation
            Dim LeftType As TypeSystem.Type = Left.GetExpressionReturnType(Context)
            Dim RightType As TypeSystem.Type = Right.GetExpressionReturnType(Context)

            Return LeftType.GetRelation(Op, {RightType}, Location)
        End Function

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type
            Return GetRelation(Context).ReturnType
        End Function

        Public Overrides Function CompileExpression(Scope As Context.Scope) As String
            Return GetRelation(Scope).CompileCall(Left, {Right}, Scope, Location)
        End Function

    End Class
End Namespace