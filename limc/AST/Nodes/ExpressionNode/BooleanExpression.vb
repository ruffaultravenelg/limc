Namespace AST
    Public Class BooleanExpression
        Inherits ExpressionNode

        Private Value As Boolean

        Public Sub New(Value As Boolean, Location As Location)
            MyBase.New(Location)
            Me.Value = Value
        End Sub

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type
            Return TypeSystem.Type.Bool
        End Function

        Public Overrides Function CompileExpression(Scope As Context.Scope) As String
            Return If(Value, "true", "false")
        End Function

    End Class
End Namespace