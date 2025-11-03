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

        Protected Overridable Function GetMatch(Context As Context.Context) As SearchMatch
            Dim ParentType As TypeSystem.Type = Parent.GetExpressionReturnType(Context)
            Dim Elements As IEnumerable(Of SearchMatch) = ParentType.RetrieveElements(ElementName)
            If Elements.Count = 0 Then
                Throw New UnknownOrUnreachableElementError(ElementName, Location)
            End If
            Return Elements.First()
        End Function

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type

            Dim Element As SearchMatch = GetMatch(Context)

            If Element.Type = SearchMatch.MatchType.MATCH_METHOD Then
                Return Element.MatchingMethod.AssociatedFunctionType
            Else
                Throw New UnknownOrUnreachableElementError(ElementName, Location)
            End If

        End Function

        Public Overrides Function CompileExpression(Scope As Context.Scope) As String

            Dim Element As SearchMatch = GetMatch(Scope)

            If Element.Type = SearchMatch.MatchType.MATCH_METHOD Then
                Return Element.MatchingMethod.AssociatedFunctionType.GetValueFromMethodNameAndInstance(Scope, Element.MatchingMethod.GeneratedFunction.CompiledName, Parent.CompileExpression(Scope), Parent.GetExpressionReturnType(Scope))
            Else
                Throw New UnknownOrUnreachableElementError(ElementName, Location)
            End If

        End Function

        Public Sub CompileAssignation(NewValue As ExpressionNode, Scope As Context.Scope) Implements IAssignable.CompileAssignation
            Throw New NotImplementedException()
        End Sub

        ' If the first element is a functions, return it (called by FunctionCallExpression to avoid wrapping a function)
        Public Function TryGetReferencedMethod(Context As Context.Context) As Lazy.Method Implements IMethodReference.TryGetReferencedMethod

            Dim ParentType As TypeSystem.Type = Parent.GetExpressionReturnType(Context)
            Dim Elements As IEnumerable(Of SearchMatch) = ParentType.RetrieveElements(ElementName).Where(Function(m) m.Type = SearchMatch.MatchType.MATCH_METHOD)

            If Elements.Count > 0 Then
                Return Elements.First().MatchingMethod
            Else
                Return Nothing
            End If

        End Function

        Function GetInstanceExpression() As ExpressionNode Implements IMethodReference.GetInstanceExpression
            Return Parent
        End Function

    End Class
End Namespace