Namespace AST
    Public Class ElementExpression
        Inherits ExpressionNode
        Implements IAssignable, IFunctionReference

        Protected ElementName As String

        Public Sub New(ElementName As String, Location As Location)
            MyBase.New(Location)
            Me.ElementName = ElementName
        End Sub

        Protected Overridable Function GetMatchs(Context As Context.Context) As IEnumerable(Of SearchMatch)
            Dim Matches = Context.RetrieveMatchingElements(ElementName, {})
            If Matches.Count = 0 Then
                Throw New UnknownOrUnreachableElementError(ElementName, Location)
            End If
            Return Matches
        End Function

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type

            Dim Element As SearchMatch = GetMatchs(Context).First()

            If Element.Type = SearchMatch.MatchType.MATCH_VARIABLE Then
                Return Element.MatchingVariable.Type
            ElseIf Element.Type = SearchMatch.MatchType.MATCH_CONSTANT Then
                Return Element.MatchingConstant.Type
            ElseIf Element.Type = SearchMatch.MatchType.MATCH_FUNCTION Then
                Return Element.MatchingFunction.AssociatedFunctionType
            ElseIf Element.Type = SearchMatch.MatchType.MATCH_SCOPE_GETTER Then
                Return Element.MatchingScopeGetter.Type
            Else
                Throw New UnknownOrUnreachableElementError(ElementName, Location)
            End If

        End Function

        Public Overrides Function CompileExpression(Writer As CWriter, Scope As Context.Scope) As String

            Dim Element As SearchMatch = GetMatchs(Scope).First()

            If Element.Type = SearchMatch.MatchType.MATCH_VARIABLE Then
                Return Element.MatchingVariable.CompiledName
            ElseIf Element.Type = SearchMatch.MatchType.MATCH_CONSTANT Then
                Return Element.MatchingConstant.CompiledName
            ElseIf Element.Type = SearchMatch.MatchType.MATCH_FUNCTION Then
                Return Element.MatchingFunction.CompileFunctionPointer()
            ElseIf Element.Type = SearchMatch.MatchType.MATCH_SCOPE_GETTER Then
                Return Element.MatchingScopeGetter.CompileCall()
            Else
                Throw New UnknownOrUnreachableElementError(ElementName, Location)
            End If

        End Function

        Protected Overrides Function _CompileAsLValue(Writer As CWriter, Scope As Context.Scope) As String

            Dim Element As SearchMatch = GetMatchs(Scope).First()

            If Element.Type = SearchMatch.MatchType.MATCH_VARIABLE Then
                Return $"(&{Element.MatchingVariable.CompiledName})"
            ElseIf Element.Type = SearchMatch.MatchType.MATCH_CONSTANT Then
                Return $"(&{Element.MatchingConstant.CompiledName})"
            ElseIf Element.Type = SearchMatch.MatchType.MATCH_SCOPE_GETTER Then
                Return Element.MatchingScopeGetter.CallGetterButReturnsValuePointer(Location)
            Else
                Throw New ExpressionDoesNotReferToAVariableError(Location)
            End If

        End Function

        Public Sub CompileAssignation(NewValue As ExpressionNode, Writer As CWriter, Scope As Context.Scope) Implements IAssignable.CompileAssignation

            ' Get value
            Dim Result As SearchMatch = GetMatchs(Scope).Where(Function(Match) Match.Type = SearchMatch.MatchType.MATCH_VARIABLE OrElse Match.Type = SearchMatch.MatchType.MATCH_SCOPE_SETTER).FirstOrDefault()
            If Result Is Nothing Then
                Throw New UnknownOrUnreachableElementError(ElementName, Location)
            End If

            If Result.Type = SearchMatch.MatchType.MATCH_VARIABLE Then

                'Get variable
                Dim Variable As VariableData = Result.MatchingVariable

                'Check type error
                If Variable.Type <> NewValue.GetExpressionReturnType(Scope) Then
                    Throw New TypeMismatchError(Variable.Type, NewValue.GetExpressionReturnType(Scope), NewValue.Location)
                End If

                'Write assignment
                Variable.Type.SetVariableValue(Writer, Variable.CompiledName, NewValue.CompileExpression(Writer, Scope))


            ElseIf Result.Type = SearchMatch.MatchType.MATCH_SCOPE_SETTER Then

                'Get setter
                Dim Setter As ScopeSetter = Result.MatchingScopeSetter

                'Check type error
                If Setter.Type <> NewValue.GetExpressionReturnType(Scope) Then
                    Throw New TypeMismatchError(Setter.Type, NewValue.GetExpressionReturnType(Scope), NewValue.Location)
                End If

                'Write assignment
                Setter.CompileCall(Writer, NewValue.CompileExpression(Writer, Scope))

            Else
                Throw New InternalError()

            End If

        End Sub

        ' If the first element is a functions, return it (called by FunctionCallExpression to avoid wrapping a function)
        Public Function TryGetReferencedFunction(Context As Context.Context) As Lazy.Function Implements IFunctionReference.TryGetReferencedFunction

            Dim Element As SearchMatch = GetMatchs(Context).First()
            If Element.Type = SearchMatch.MatchType.MATCH_FUNCTION Then
                Return Element.MatchingFunction
            Else
                Return Nothing
            End If

        End Function

    End Class
End Namespace