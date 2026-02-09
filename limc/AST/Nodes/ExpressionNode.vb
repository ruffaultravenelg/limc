Namespace AST
    Public MustInherit Class ExpressionNode
        Inherits AST.Node

        Public Sub New(Location As Location)
            MyBase.New(Location)
        End Sub

        Public MustOverride Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type
        Public MustOverride Function CompileExpression(Writer As CWriter, Scope As Context.Scope) As String

        Public Function CompileExpressionAsLValue(Writer As CWriter, Scope As Context.Scope) As String
            If GetExpressionReturnType(Scope).IsPointer Then
                Return CompileExpression(Writer, Scope)
            Else
                Return _CompileAsLValue(Writer, Scope)
            End If
        End Function

        ' Will only be called when value is not a pointer
        Protected Overridable Function _CompileAsLValue(Writer As CWriter, Scope As Context.Scope) As String
            Throw New ExpressionDoesNotReferToAVariableError(Location)
        End Function

    End Class
End Namespace