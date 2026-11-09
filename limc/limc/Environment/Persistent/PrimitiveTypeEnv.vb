Imports limc.Symbol

Namespace Environment
    Friend Class PrimitiveTypeEnv
        Inherits TypeEnvironment

        Private MethodRepo As Repository.Primitive.MethodRepository

        Private Sub New(Parent As Env, Methods As IReadOnlyList(Of Symbol.PrimitiveMethodSymbol))
            MyBase.New(Parent)
            Me.MethodRepo = New Repository.Primitive.MethodRepository(Methods)
        End Sub

        Protected Overrides Iterator Function ResolveSymbol(ElementName As String, PassedGenericTypes As IReadOnlyList(Of Symbol.TypeSymbol)) As IEnumerable(Of SymbolMatch)
            If PassedGenericTypes.Count = 0 Then
                For Each Res In MethodRepo.RetrieveMethodSymbols(ElementName)
                    Yield New SymbolMatch(Res)
                Next
            End If
        End Function

        Protected Overrides Function ResolveType(Typename As String, PassedGenericTypes As IReadOnlyList(Of Symbol.TypeSymbol)) As Symbol.TypeSymbol
            Return Nothing
        End Function

        ' ==== CONSTRUCTOR ====
        Public Class Builder

            Private Parent As Env = Nothing
            Private Methods As New List(Of PrimitiveMethodSymbol)

            Public Function SetParentCtx(Parent As Env) As Builder
                Me.Parent = Parent
                Return Me
            End Function

            Public Function AddMethod(Method As PrimitiveMethodSymbol) As Builder
                Me.Methods.Add(Method)
                Return Me
            End Function

            Public Function Build() As PrimitiveTypeEnv
                Return New PrimitiveTypeEnv(Parent, Methods)
            End Function

        End Class

    End Class
End Namespace