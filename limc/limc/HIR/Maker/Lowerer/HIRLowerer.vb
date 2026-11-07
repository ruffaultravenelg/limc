Imports limc.Symbol

Namespace HIR.Lowerer
    Friend Class HIRLowerer
        Implements IHIRLowerer

        Private WorkQueue As CompilationQueue
        Private Primitives As IPrimitives
        Private StatementCompiler As StatementCompiler
        Private ExpressionCompiler As ExpressionCompiler

        Public Sub New(WorkQueue As CompilationQueue, Primitives As IPrimitives, FileEnvs As Dictionary(Of AST.FileNode, Environment.FileEnvironment))
            Me.WorkQueue = WorkQueue
            Me.Primitives = Primitives
            Me.ExpressionCompiler = New ExpressionCompiler(WorkQueue, Primitives)
            Me.StatementCompiler = New StatementCompiler(WorkQueue, ExpressionCompiler)
        End Sub

        ' ==== COMPILE FUNCTION ====
        Public Function CompileUserFunction(FuncSymbol As UserFunctionSymbol) As FunctionConstruct Implements IHIRLowerer.CompileUserFunction

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