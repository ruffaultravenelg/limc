Namespace C.Naming
    Friend Interface ISymbolNamer
        Function GetCompiledName(Symbol As Symbol.Symbol) As String
        Sub BindKnownSymbol(Symbol As Symbol.Symbol, CompiledName As String)

    End Interface
End Namespace