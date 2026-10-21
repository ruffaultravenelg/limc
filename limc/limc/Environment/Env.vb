Imports limc.Symbol

Namespace Environment
    Friend MustInherit Class Env

        Private EnclosingEnvironment As Env

        Public Sub New(_EnclosingEnvironment As Env)
            Me.EnclosingEnvironment = _EnclosingEnvironment
        End Sub

        Private Iterator Function AllParents() As IEnumerable(Of Env)
            Dim Current As Env = Me
            While Current IsNot Nothing
                Yield Current
                Current = Current.EnclosingEnvironment
            End While
        End Function

        ' Symbol search API
        Public Iterator Function FindSymbol(Of S As Symbol.Symbol)(ElementName As String, PassedGenericTypes As IReadOnlyList(Of Symbol.TypeSymbol)) As IEnumerable(Of S)
            For Each Match In FindSymbol(ElementName, PassedGenericTypes)
                If Match.IsType(Of S) Then
                    Yield Match.CastAs(Of S)
                End If
            Next
        End Function
        Public Iterator Function FindSymbol(ElementName As String, PassedGenericTypes As IReadOnlyList(Of Symbol.TypeSymbol)) As IEnumerable(Of SymbolMatch)
            For Each Parent In AllParents()
                For Each Match In Parent.ResolveSymbol(ElementName, PassedGenericTypes)
                    Yield Match
                Next
            Next
        End Function
        Protected MustOverride Iterator Function ResolveSymbol(ElementName As String, PassedGenericTypes As IReadOnlyList(Of Symbol.TypeSymbol)) As IEnumerable(Of SymbolMatch)

        ' Type search API
        Public Function FindType(Typename As String, PassedGenericTypes As IReadOnlyList(Of TypeSymbol)) As TypeSymbol
            For Each Parent In AllParents()
                Dim Res = Parent.ResolveType(Typename, PassedGenericTypes)
                If Res IsNot Nothing Then
                    Return Res
                End If
            Next
            Return Nothing
        End Function
        Protected MustOverride Function ResolveType(Typename As String, PassedGenericTypes As IReadOnlyList(Of TypeSymbol)) As TypeSymbol

    End Class
End Namespace