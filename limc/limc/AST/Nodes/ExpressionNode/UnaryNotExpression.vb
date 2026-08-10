Namespace AST
    Public Class UnaryNotExpression
        Inherits ExpressionNode

        Private Value As ExpressionNode

        Public Sub New(Value As ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.Value = Value
        End Sub

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type
            Return TypeSystem.Type.Bool
        End Function

        Public Overrides Function CompileExpression(Writer As CWriter, Scope As Context.Scope) As String
            Dim ValueType As TypeSystem.Type = Value.GetExpressionReturnType(Scope)
            If ValueType <> TypeSystem.Type.Bool Then
                Throw New TypeMismatchError(TypeSystem.Type.Bool, ValueType, Value.Location)
            End If

            Return $"!({Value.CompileExpression(Writer, Scope)})"

        End Function

    End Class
End Namespace