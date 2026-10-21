Namespace Symbol
    Friend Class IntTypeSymbol
        Inherits TypeSymbol

        Public Sub New()
            MyBase.New("int")
        End Sub

        Public Overrides Sub Compile(Lowerer As HIR.Lowerer.IHIRLowerer, ProgramBuilder As HIR.ProgramNode.Builder)
            ' Do nothing => int will be compiled by default with Lim C runtime
        End Sub

    End Class
End Namespace