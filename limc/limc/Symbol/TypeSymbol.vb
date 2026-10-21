Imports limc.HIR
Imports limc.HIR.Lowerer

Namespace Symbol
    Friend MustInherit Class TypeSymbol
        Inherits Symbol
        Implements ICompilableSymbol

        Public Sub New(LimName As String)
            MyBase.New(LimName)
        End Sub

        ' ICompilable stuff
        Public Property Compiled As Boolean = False Implements ICompilableSymbol.Compiled
        Public MustOverride Sub Compile(Lowerer As IHIRLowerer, ProgramBuilder As ProgramNode.Builder) Implements ICompilableSymbol.Compile

        ' ==== SHARED MAIN TYPES ====
        Public Shared ReadOnly INT As New IntTypeSymbol()

    End Class
End Namespace