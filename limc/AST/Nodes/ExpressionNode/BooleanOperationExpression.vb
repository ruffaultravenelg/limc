Namespace AST
    Public Class BooleanOperationExpression
        Inherits ExpressionNode

        Private Left As ExpressionNode
        Private Op As TokenType
        Private Right As ExpressionNode

        Public Sub New(Left As ExpressionNode, Op As TokenType, Right As ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.Left = Left
            Me.Op = Op
            Me.Right = Right
        End Sub

        Private Sub CheckTypes(Context As Context.Context)
            Dim LeftType As TypeSystem.Type = Left.GetExpressionReturnType(Context)
            If LeftType <> TypeSystem.Type.Bool Then
                Throw New TypeMismatchError(TypeSystem.Type.Bool, LeftType, Left.Location)
            End If

            Dim RightType As TypeSystem.Type = Right.GetExpressionReturnType(Context)
            If RightType <> TypeSystem.Type.Bool Then
                Throw New TypeMismatchError(TypeSystem.Type.Bool, RightType, Right.Location)
            End If
        End Sub

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type
            CheckTypes(Context)
            Return TypeSystem.Type.Bool
        End Function

        Public Overrides Function CompileExpression(Writer As CWriter, Scope As Context.Scope) As String
            CheckTypes(Scope)

            Select Case Op
                Case TokenType.KEYWORD_AND
                    Return $"({Left.CompileExpression(Writer, Scope)} && {Right.CompileExpression(Writer, Scope)})"
                Case TokenType.KEYWORD_OR
                    Return $"({Left.CompileExpression(Writer, Scope)} || {Right.CompileExpression(Writer, Scope)})"
                Case Else
                    Throw New InternalError()
            End Select

        End Function

    End Class
End Namespace