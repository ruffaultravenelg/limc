Namespace Environment
    Friend Class PrimitiveTypeEnv
        Inherits Env

        Private Sub New(Parent As Env)
            MyBase.New(Parent)
        End Sub

        Protected Overrides Iterator Function ResolveSymbol(ElementName As String, PassedGenericTypes As IReadOnlyList(Of Symbol.TypeSymbol)) As IEnumerable(Of SymbolMatch)

        End Function

        Protected Overrides Function ResolveType(Typename As String, PassedGenericTypes As IReadOnlyList(Of Symbol.TypeSymbol)) As Symbol.TypeSymbol
            Return Nothing
        End Function

        ' ==== CONSTRUCTOR ====
        Public Class Maker

            Private Parent As Env = Nothing

            Public Function SetParentCtx(Parent As Env) As Maker
                Me.Parent = Parent
                Return Me
            End Function

            Public Function Build() As PrimitiveTypeEnv
                Return New PrimitiveTypeEnv(Parent)
            End Function

        End Class

    End Class
End Namespace