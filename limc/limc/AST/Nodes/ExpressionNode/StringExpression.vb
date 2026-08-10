Namespace AST
    Public Class StringExpression
        Inherits ExpressionNode
        Implements IConstantExpression

        Private Value As String
        Public Sub New(Value As String, Location As Location)
            MyBase.New(Location)
            Me.Value = Value
        End Sub

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type
            Return TypeSystem.Type.Str
        End Function

        Public Overrides Function CompileExpression(Writer As CWriter, Scope As Context.Scope) As String
            Return """" & CodeGen.Sanitize(Value) & """"
        End Function

    End Class
End Namespace