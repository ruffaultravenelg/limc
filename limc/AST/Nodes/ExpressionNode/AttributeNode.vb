Namespace AST
    Public Class AttributeExpression
        Inherits ExpressionNode
        Implements IAssignable, IMethodReference

        Private Parent As ExpressionNode
        Private ElementName As String

        Public Sub New(Parent As ExpressionNode, ElementName As String, Location As Location)
            MyBase.New(Location)
            Me.Parent = Parent
            Me.ElementName = ElementName
        End Sub

        Protected Overridable Function GetMatchs(Context As Context.Context) As IEnumerable(Of SearchMatch)
            Dim ParentType As TypeSystem.Type = Parent.GetExpressionReturnType(Context)
            Dim Elements As IEnumerable(Of SearchMatch) = ParentType.RetrieveElementsFromOutside(ElementName, {})
            If Elements.Count = 0 Then
                Throw New UnknownOrUnreachableElementError(ElementName, Location)
            End If
            Return Elements
        End Function

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type

            Dim Element As SearchMatch = GetMatchs(Context).First()

            If Element.Type = SearchMatch.MatchType.MATCH_METHOD Then
                Return Element.MatchingMethod.AssociatedFunctionType
            ElseIf Element.Type = SearchMatch.MatchType.MATCH_GETTER Then
                Return Element.MatchingGetter.Type
            Else
                Throw New UnknownOrUnreachableElementError(ElementName, Location)
            End If

        End Function

        Public Overrides Function CompileExpression(Writer As CWriter, Scope As Context.Scope) As String

            Dim Element As SearchMatch = GetMatchs(Scope).First()

            If Element.Type = SearchMatch.MatchType.MATCH_METHOD Then
                Throw New LocatedError("Methods cannot be referenced for now", "referencing object methods can lead to double pointer variable, breaking tgc", Location)
            ElseIf Element.Type = SearchMatch.MatchType.MATCH_GETTER Then
                Return Element.MatchingGetter.CallGetter(Parent.CompileExpression(Writer, Scope))
            Else
                Throw New UnknownOrUnreachableElementError(ElementName, Location)
            End If

        End Function

        Public Overrides Function GetPointerToValue(Writer As CWriter, Scope As Context.Scope) As String

            Dim Element As SearchMatch = GetMatchs(Scope).First()
            Dim ParentType As TypeSystem.Type = Parent.GetExpressionReturnType(Scope)

            If Element.Type = SearchMatch.MatchType.MATCH_GETTER Then
                If Element.MatchingGetter.Type.IsPointer Then
                    Return Element.MatchingGetter.CallGetter(Parent.CompileExpression(Writer, Scope))
                Else
                    Return Element.MatchingGetter.GetReference(Parent.GetPointerToValue(Writer, Scope), Location)
                End If
            Else
                Throw New ExpressionDoesNotReferToAVariableError(Location)
            End If

        End Function

        Public Sub CompileAssignation(NewValue As ExpressionNode, Writer As CWriter, Scope As Context.Scope) Implements IAssignable.CompileAssignation

            Dim MatchingSetters = GetMatchs(Scope).Where(Function(elm) elm.Type = SearchMatch.MatchType.MATCH_SETTER)
            Dim NewValueType As TypeSystem.Type = NewValue.GetExpressionReturnType(Scope)

            For Each Match In MatchingSetters
                If Match.MatchingSetter.Type = NewValueType Then
                    If NewValueType.IsPointer Then
                        Match.MatchingSetter.WriteSetterCall(Writer, Parent.CompileExpression(Writer, Scope), NewValue.CompileExpression(Writer, Scope))
                    Else
                        Match.MatchingSetter.WriteSetterCall(Writer, Parent.GetPointerToValue(Writer, Scope), NewValue.CompileExpression(Writer, Scope))
                    End If

                    Exit Sub
                End If
            Next

            Throw New UnknownOrUnreachableElementError(ElementName, Location)

        End Sub

        ' If the first element is a functions, return it (called by FunctionCallExpression to avoid wrapping a function)
        Public Function TryGetReferencedMethod(Context As Context.Context) As Lazy.Method Implements IMethodReference.TryGetReferencedMethod

            Dim Element As SearchMatch = GetMatchs(Context).First()
            If Element.Type = SearchMatch.MatchType.MATCH_METHOD Then
                Return Element.MatchingMethod
            Else
                Return Nothing
            End If

        End Function

        Function GetInstanceExpression() As ExpressionNode Implements IMethodReference.GetInstanceExpression
            Return Parent
        End Function

    End Class
End Namespace