Namespace AST
    Public Class IntExpression
        Inherits ExpressionNode
        Implements IConstantExpression

        Private Value As Integer
        Public Sub New(Value As Integer, Location As Location)
            MyBase.New(Location)
            Me.Value = Value
        End Sub

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type
            Return TypeSystem.Type.Int
        End Function

        Public Overrides Function CompileExpression(Scope As Context.Scope) As String
            Return Value.ToString()
        End Function

    End Class
End Namespace