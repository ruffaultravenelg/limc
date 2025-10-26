Namespace AST
    Public Class ElementExpression
        Inherits ExpressionNode
        Implements IAssignable

        Private ElementName As String

        Public Sub New(ElementName As String, Location As Location)
            MyBase.New(Location)
            Me.ElementName = ElementName
        End Sub

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type
            Throw New NotImplementedException()
        End Function

        Public Overrides Function CompileExpression(Scope As Context.Scope) As String
            Throw New NotImplementedException()
        End Function

        Public Sub CompileAssignation(NewValue As ExpressionNode, Scope As Context.Scope) Implements IAssignable.CompileAssignation

            'Search variable
            Dim Variable As VariableData = Scope.GetVariable(ElementName, Location)

            'Check type error
            If Variable.Type <> NewValue.GetExpressionReturnType(Scope) Then
                Throw New TypeMismatchError(Variable.Type, NewValue.GetExpressionReturnType(Scope), Location)
            End If

            'Write assignment
            Variable.Type.SetVariableValue(Scope, Variable.CompiledName, NewValue.CompileExpression(Scope))

        End Sub

    End Class
End Namespace