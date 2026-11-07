Imports limc.C.Naming
Imports limc.Environment

Friend Class PrimitiveCompiledNamePopulator

    Public Shared Sub PopulateNamer(Namer As ISymbolNamer)
        Namer.BindKnownSymbol(Primitives.INT, "int")
        Namer.BindKnownSymbol(Primitives.FLOAT, "double")
        Namer.BindKnownSymbol(Primitives.BOOL, "bool")
        Namer.BindKnownSymbol(Primitives.STR, "lim_str")
    End Sub

End Class
