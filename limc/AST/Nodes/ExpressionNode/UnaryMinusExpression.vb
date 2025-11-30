Imports limc.TypeSystem.Type
Namespace AST
    Public Class UnaryMinusExpression
        Inherits ExpressionNode

        Private Value As ExpressionNode

        Public Sub New(Value As ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.Value = Value
        End Sub

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type
            Dim ValueType As TypeSystem.Type = Value.GetExpressionReturnType(Context)

            If ValueType Is Int Then
                Return Int
            ElseIf ValueType Is Float Then
                Return Float
            Else
                Return ValueType.GetRelation(TypeSystem.RelationType.RELATION_UNARY_MINUS, {}, Location).ReturnType
            End If
        End Function

        Public Overrides Function CompileExpression(Scope As Context.Scope) As String
            Dim ValueType As TypeSystem.Type = Value.GetExpressionReturnType(Scope)

            If ValueType Is Int OrElse ValueType Is Float Then
                Return $"(-{Value.CompileExpression(Scope)})"
            Else
                Return ValueType.GetRelation(TypeSystem.RelationType.RELATION_UNARY_MINUS, {}, Location).CompileCall(Value, {}, Scope, Location)
            End If
        End Function

    End Class
End Namespace