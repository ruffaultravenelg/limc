Namespace AST
    Public Class GenericElementExpression
        Inherits ExpressionNode
        Implements IFunctionReference, IEnumReference

        Protected ElementName As String
        Protected PassedGenericTypes As IEnumerable(Of TypeNode)

        Public Sub New(ElementName As String, PassedGenericTypes As IEnumerable(Of TypeNode), Location As Location)
            MyBase.New(Location)
            Me.ElementName = ElementName
            Me.PassedGenericTypes = PassedGenericTypes
        End Sub

        Protected Overridable Function GetMatch(Context As Context.Context) As SearchMatch
            Return Context.RetrieveMatchingElements(ElementName, PassedGenericTypes.Select(Function(g) g.GetAssociatedType(Context))).First() 'TODO: .First() is weird
        End Function

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type

            Dim Element As SearchMatch = GetMatch(Context)

            If Element.Type = SearchMatch.MatchType.MATCH_FUNCTION Then
                Return Element.MatchingFunction.AssociatedFunctionType
            Else
                Throw New UnknownOrUnreachableElementError(ElementName, Location)
            End If

        End Function

        Public Overrides Function CompileExpression(Writer As CWriter, Scope As Context.Scope) As String

            Dim Element As SearchMatch = GetMatch(Scope)

            If Element.Type = SearchMatch.MatchType.MATCH_FUNCTION Then
                Return Element.MatchingFunction.CompileFunctionPointer()
            Else
                Throw New UnknownOrUnreachableElementError(ElementName, Location)
            End If

        End Function

        ' If the first element is a functions, return it (called by FunctionCallExpression to avoid wrapping a function)
        Public Function TryGetReferencedFunction(Context As Context.Context) As Lazy.Function Implements IFunctionReference.TryGetReferencedFunction

            Dim Element As SearchMatch = GetMatch(Context)
            If Element.Type = SearchMatch.MatchType.MATCH_FUNCTION Then
                Return Element.MatchingFunction
            Else
                Return Nothing
            End If

        End Function

        Protected Overridable Function TryGetEnumReference(Context As Context.Context) As TypeSystem.EnumType Implements IEnumReference.TryGetEnumReference

            Dim GenericTypes = PassedGenericTypes.Select(Function(g) g.GetAssociatedType(Context))
            Dim RetrievedType = Context.ParentFile.RetrieveTypeFromLocal(ElementName, GenericTypes)
            If TypeOf RetrievedType Is TypeSystem.EnumType Then
                Return RetrievedType
            Else
                Return Nothing
            End If

        End Function

    End Class
End Namespace