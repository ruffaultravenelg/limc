Imports limc.HIR.Lowerer

Namespace Symbol
    Friend Class FunctionSymbol
        Inherits Symbol
        Implements ICompilableSymbol

        Public ReadOnly Property Node As AST.FunctionConstruct
        Public ReadOnly Property PassedGeneric As IReadOnlyList(Of TypeSymbol)
        Public ReadOnly Property Arguments As IReadOnlyList(Of VariableSymbol)
        Public ReadOnly Property ReturnType As TypeSymbol
        Public ReadOnly Property FunctionEnvironment As Environment.Env

        Public Sub New(Node As AST.FunctionConstruct, PassedGeneric As IEnumerable(Of TypeSymbol), Arguments As IEnumerable(Of VariableSymbol), ReturnType As TypeSymbol, FuncEnv As Environment.Env)
            MyBase.New(Node.Name)
            Me.Node = Node
            Me.PassedGeneric = PassedGeneric.ToList()
            Me.Arguments = Arguments.ToList()
            Me.ReturnType = ReturnType
            Me.FunctionEnvironment = FuncEnv
        End Sub

        Public Property Compiled As Boolean = False Implements ICompilableSymbol.Compiled

        Public Sub Compile(Lowerer As IHIRLowerer, ProgramBuilder As HIR.ProgramNode.Builder) Implements ICompilableSymbol.Compile
            Dim CompiledFunc = Lowerer.CompileFunction(Me)
            ProgramBuilder.AppendFunction(CompiledFunc)
        End Sub

    End Class
End Namespace