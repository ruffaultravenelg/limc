Namespace Symbol
    Friend Class IntTypeSymbol
        Inherits TypeSymbol

        Public Sub New()
            MyBase.New("int")
        End Sub

        Public Overrides Sub Compile(Lowerer As HIR.Lowerer.IHIRLowerer, ProgramBuilder As HIR.ProgramNode.Builder)
            ' Do nothing => int will be compiled by default with Lim C runtime
        End Sub

        Public Overrides Function ToString() As String
            Return "int"
        End Function

    End Class
End Namespace