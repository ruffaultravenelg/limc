Imports limc.HIR.Lowerer

Namespace Symbol
    Friend MustInherit Class FunctionSymbol
        Inherits Symbol
        Implements ICompilableSymbol

        Public ReadOnly Property PassedGeneric As IReadOnlyList(Of TypeSymbol)
        Public ReadOnly Property Arguments As IReadOnlyList(Of VariableSymbol)
        Public ReadOnly Property ReturnType As TypeSymbol

        Public Sub New(LimName As String, PassedGeneric As IEnumerable(Of TypeSymbol), Arguments As IEnumerable(Of VariableSymbol), ReturnType As TypeSymbol)
            MyBase.New(LimName)
            Me.PassedGeneric = PassedGeneric.ToList()
            Me.Arguments = Arguments.ToList()
            Me.ReturnType = ReturnType
        End Sub

        Public Property Compiled As Boolean = False Implements ICompilableSymbol.Compiled
        Public MustOverride Sub Compile(Lowerer As IHIRLowerer, ProgramBuilder As HIR.ProgramNode.Builder) Implements ICompilableSymbol.Compile

    End Class
End Namespace