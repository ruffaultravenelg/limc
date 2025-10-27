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

            Dim Element As SearchMatch = Context.RetrieveMatchingElement(ElementName, Location)

            If Element.Type = SearchMatch.MatchType.MATCH_VARIABLE Then
                Return Element.MatchingVariable.Type
            ElseIf Element.Type = SearchMatch.MatchType.MATCH_FUNCTION Then
                Return Element.MatchingFunction.FuncScope.AssociatedFunctionType
            Else
                Throw New UnknownOrUnreachableElementError(ElementName, Location)
            End If

        End Function

        Public Overrides Function CompileExpression(Scope As Context.Scope) As String

            Dim Element As SearchMatch = Scope.RetrieveMatchingElement(ElementName, Location)

            If Element.Type = SearchMatch.MatchType.MATCH_VARIABLE Then
                Return Element.MatchingVariable.CompiledName
            ElseIf Element.Type = SearchMatch.MatchType.MATCH_FUNCTION Then
                Return Element.MatchingFunction.FuncScope.AssociatedFunctionType.GetValueFromFunctionName(Element.MatchingFunction.FuncScope.GeneratedFunction.CompiledName)
            Else
                Throw New UnknownOrUnreachableElementError(ElementName, Location)
            End If

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