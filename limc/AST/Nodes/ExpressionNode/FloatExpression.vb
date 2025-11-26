Namespace AST
    Public Class FloatExpression
        Inherits ExpressionNode
        Implements IConstantExpression

        Private Value As Double
        Public Sub New(Value As Double, Location As Location)
            MyBase.New(Location)
            Me.Value = Value
        End Sub

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type
            Return TypeSystem.Type.Float
        End Function

        Public Overrides Function CompileExpression(Scope As Context.Scope) As String
            Dim Value_STR As String = Value.ToString().Replace(",", ".")
            If Value_STR.Contains(".") Then
                Return Value_STR
            Else
                Return Value_STR & ".0"
            End If
        End Function

    End Class
End Namespace