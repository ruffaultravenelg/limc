Imports limc.HIR.Lowerer

Namespace Symbol
    Friend Class FunctionSymbol
        Inherits Symbol
        Implements ICompilableSymbol

        Public ReadOnly Property Node As AST.FunctionConstruct
        Public ReadOnly Property PassedGeneric As IReadOnlyList(Of TypeSymbol)
        Public ReadOnly Property DeclaringEnvironment As Environment.Env

        Public Sub New(Node As AST.FunctionConstruct, PassedGeneric As IEnumerable(Of TypeSymbol), DeclaringEnvironment As Environment.Env)
            MyBase.New(Node.Name)
            Me.Node = Node
            Me.PassedGeneric = PassedGeneric
            Me.DeclaringEnvironment = DeclaringEnvironment
        End Sub

        Public Property Compiled As Boolean = False Implements ICompilableSymbol.Compiled

        Public Sub Compile(Lowerer As IHIRLowerer, ProgramBuilder As HIR.ProgramNode.Builder) Implements ICompilableSymbol.Compile
            Dim CompiledFunc = Lowerer.CompileFunction(Me)
            ProgramBuilder.AppendFunction(CompiledFunc)
        End Sub

    End Class
End Namespace