Imports limc.Symbol

Namespace Repository.Primitive
    Friend Class MethodRepository

        Private SymbolTable As New Dictionary(Of String, IReadOnlyList(Of MethodSymbol))
        ' SymbolTable:
        '   "method" => [ func(a:bool), func(a:int) ]

        Public Sub New(MethodSymbols As IReadOnlyList(Of PrimitiveMethodSymbol))

            ' [methods] => { "name" => [methods], "name" => [methods], ... }
            For Each Meth In MethodSymbols
                If SymbolTable.ContainsKey(Meth.LimName) Then
                    DirectCast(SymbolTable(Meth.LimName), List(Of MethodSymbol)).Add(Meth)
                Else
                    SymbolTable(Meth.LimName) = New List(Of MethodSymbol) From {Meth}
                End If
            Next

        End Sub

        Public Function RetrieveMethodSymbols(Name As String) As IReadOnlyList(Of MethodSymbol)
            If SymbolTable.ContainsKey(Name) Then
                Return SymbolTable(Name)
            Else
                Return Array.Empty(Of MethodSymbol)
            End If
        End Function

    End Class
End Namespace