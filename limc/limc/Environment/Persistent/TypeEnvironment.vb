Namespace Environment
    Friend Class TypeEnvironment
        Inherits Env

        Public Sub New(Parent As Env)
            MyBase.New(Parent)
        End Sub

        ' method repo
        ' relation repo

        Protected Overrides Function ResolveSymbol(ElementName As String, PassedGenericTypes As IReadOnlyList(Of Symbol.TypeSymbol)) As IEnumerable(Of SymbolMatch)
            Throw New NotImplementedException()
        End Function

        Protected Overrides Function ResolveType(Typename As String, PassedGenericTypes As IReadOnlyList(Of Symbol.TypeSymbol)) As Symbol.TypeSymbol
            Throw New NotImplementedException()
        End Function

        Public Overridable Function GetRelation()
            Throw New NotImplementedException()
        End Function

    End Class
End Namespace