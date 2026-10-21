Namespace Symbol
    Friend Interface ICompilableSymbol
        Property Compiled As Boolean
        Sub Compile(Lowerer As HIR.Lowerer.IHIRLowerer, ProgramBuilder As HIR.ProgramNode.Builder)

    End Interface
End Namespace