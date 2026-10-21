Imports System.Reflection.Metadata.Ecma335
Imports limc.Symbol

Namespace Environment
    Friend Class GenericEnv
        Inherits Env

        Private GenericStore As New Dictionary(Of String, TypeSymbol)

        Public Sub New(Parent As Env)
            MyBase.New(Parent)
        End Sub

        Public Sub DefineGenericType(Typename As String, Type As TypeSymbol)
            GenericStore(Typename) = Type
        End Sub

        Protected Overrides Iterator Function ResolveSymbol(ElementName As String, PassedGenericTypes As IReadOnlyList(Of Symbol.TypeSymbol)) As IEnumerable(Of SymbolMatch)
            ' This environment doesn't contains any symbols (other than type)
        End Function

        Protected Overrides Function ResolveType(Typename As String, PassedGenericTypes As IReadOnlyList(Of TypeSymbol)) As TypeSymbol
            If PassedGenericTypes.Count = 0 AndAlso GenericStore.ContainsKey(Typename) Then
                Return GenericStore(Typename)
            Else
                Return Nothing
            End If
        End Function

    End Class
End Namespace