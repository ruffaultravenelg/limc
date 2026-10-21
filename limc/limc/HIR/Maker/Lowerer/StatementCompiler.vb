Imports limc.Environment

Namespace HIR.Lowerer
    Friend Class StatementCompiler
        Implements AST.IStatementVisitor(Of BlockEnv, StatementNode)

        Private WorkQueue As CompilationQueue
        Private ExpressionCompiler As ExpressionCompiler

        Public Sub New(WorkQueue As CompilationQueue, ExpressionCompiler As ExpressionCompiler)
            Me.WorkQueue = WorkQueue
            Me.ExpressionCompiler = ExpressionCompiler
        End Sub

        Public Function VisitDeclareVariableInferTypeStatement(Node As AST.DeclareVariableInferTypeStatement, Ctx As BlockEnv) As StatementNode Implements AST.IStatementVisitor(Of BlockEnv, StatementNode).VisitDeclareVariableInferTypeStatement
            Dim CompiledValue = Node.Value.Accept(ExpressionCompiler, Ctx)
            WorkQueue.Enqueue(CompiledValue.ExpressionReturnType)
            Dim VarSymbol = Ctx.DefineVariable(Node.VariableName, CompiledValue.ExpressionReturnType)
            Return New DeclareVariableStatement(Node.Location, VarSymbol, CompiledValue)
        End Function

        Public Function VisitBlockStatement(Node As AST.BlockStatement, Ctx As BlockEnv) As StatementNode Implements AST.IStatementVisitor(Of BlockEnv, StatementNode).VisitBlockStatement
            Return New BlockStatement(Node.Location, Node.Statements.Select(Function(n) n.Accept(Me, Ctx)))
        End Function

    End Class
End Namespace