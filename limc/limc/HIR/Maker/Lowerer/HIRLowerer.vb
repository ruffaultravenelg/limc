Imports limc.Symbol

Namespace HIR.Lowerer
    Friend Class HIRLowerer
        Implements IHIRLowerer

        Private WorkQueue As CompilationQueue
        Private StatementCompiler As StatementCompiler
        Private ExpressionCompiler As ExpressionCompiler

        Public Sub New(WorkQueue As CompilationQueue, FileEnvs As Dictionary(Of AST.FileNode, Environment.FileEnvironment))
            Me.WorkQueue = WorkQueue
            Me.ExpressionCompiler = New ExpressionCompiler(WorkQueue)
            Me.StatementCompiler = New StatementCompiler(WorkQueue, ExpressionCompiler)
        End Sub

        ' ==== COMPILE FUNCTION ====
        Public Function CompileFunction(FuncSymbol As FunctionSymbol) As FunctionConstruct Implements IHIRLowerer.CompileFunction

            ' Enqueue generics
            WorkQueue.EnqueueRange(FuncSymbol.PassedGeneric)

            ' Enqueue parameter types
            For Each arg In FuncSymbol.Arguments
                WorkQueue.Enqueue(arg.VariableType)
            Next

            ' Compile body
            Dim Body = FuncSymbol.Node.Body.Accept(StatementCompiler, FuncSymbol.FunctionEnvironment)

            ' Create the function
            Return New FunctionConstruct(FuncSymbol.Node.Location, FuncSymbol, Body)

        End Function

    End Class
End Namespace