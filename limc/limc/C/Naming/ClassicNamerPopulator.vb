Namespace C.Naming
    Friend Class ClassicNamerPopulator
        Implements INamerPopulator

        Private Primitives As IPrimitives

        Public Sub New(Primitives As IPrimitives)
            Me.Primitives = Primitives
        End Sub

        Public Sub Populate(Namer As ISymbolNamer) Implements INamerPopulator.Populate
            Namer.BindKnownSymbol(Primitives.INT, "int")
            Namer.BindKnownSymbol(Primitives.FLOAT, "double")
            Namer.BindKnownSymbol(Primitives.BOOL, "bool")
            Namer.BindKnownSymbol(Primitives.STR, "lim_str")
        End Sub

    End Class
End Namespace