Imports limc.HIR.Lowerer

Namespace Symbol
    Friend Class UserFunctionSymbol
        Inherits FunctionSymbol

        Public ReadOnly Property Node As AST.FunctionConstruct
        Public ReadOnly Property FunctionEnvironment As Environment.Env

        Public Sub New(Node As AST.FunctionConstruct, PassedGeneric As IEnumerable(Of TypeSymbol), Arguments As IEnumerable(Of VariableSymbol), ReturnType As TypeSymbol, FuncEnv As Environment.Env)
            MyBase.New(Node.Name, PassedGeneric, Arguments, ReturnType)
            Me.Node = Node
            Me.FunctionEnvironment = FuncEnv
        End Sub

        Public Overrides Sub Compile(Lowerer As IHIRLowerer, ProgramBuilder As HIR.ProgramNode.Builder)
            Dim CompiledFunc = Lowerer.CompileUserFunction(Me)
            ProgramBuilder.AppendFunction(CompiledFunc)
        End Sub

    End Class
End Namespace