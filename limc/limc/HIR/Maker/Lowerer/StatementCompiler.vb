Imports limc.Environment
Imports limc.Symbol

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

        Public Function VisitProcedureCallStatement(Node As AST.ProcedureCallStatement, Ctx As BlockEnv) As StatementNode Implements AST.IStatementVisitor(Of BlockEnv, StatementNode).VisitProcedureCallStatement
            'IMPORTANT: If edit => edit also ExpressionCompiler.VisitProcedureCallExpression

            ' Compiled arguments
            Dim CompiledArguments As New List(Of ExpressionNode)
            For Each arg In Node.PassedArguments
                CompiledArguments.Add(arg.Accept(ExpressionCompiler, Ctx)) 'each argument will compile it's types
            Next

            ' Child may need passed arguments types to resolve which function to choose (due to function overloading)
            ' this env help to pass them this data
            Dim HintEnv As New CallHintEnv(Ctx, CompiledArguments.Select(Function(arg) arg.ExpressionReturnType))

            ' Compile procedure with hint
            Dim CompiledProcedure As ExpressionNode = Node.Procedure.Accept(ExpressionCompiler, HintEnv)
            If TypeOf CompiledProcedure.ExpressionReturnType IsNot FuncTypeSymbol Then
                Throw New TypeMismatchException("HSC01", CompiledProcedure.ExpressionReturnType, "whereas a ""func"" type was expected.", CompiledProcedure.Location)
            End If

            ' Create call
            Return New ProcedureCallStatement(Node.Location, CompiledProcedure, CompiledArguments)

        End Function

        Public Function VisitVariableAssignStatement(Node As AST.VariableAssignStatement, Ctx As BlockEnv) As StatementNode Implements AST.IStatementVisitor(Of BlockEnv, StatementNode).VisitVariableAssignStatement

            ' Get variable symbol
            Dim VarSym As VariableSymbol = Ctx.FindSymbol(Of VariableSymbol)(Node.Variable.ElementName, {}).FirstOrDefault()
            If VarSym Is Nothing Then
                Throw New UnreachableElementException("HSC02", Node.Variable.ElementName, Node.Variable.Location)
            End If
            WorkQueue.Enqueue(VarSym.VariableType)

            ' New value
            Dim NewValue = Node.NewValue.Accept(ExpressionCompiler, Ctx)

            ' Check NewValue type
            If NewValue.ExpressionReturnType <> VarSym.VariableType Then
                Throw New TypeMismatchException("HSC03", NewValue.ExpressionReturnType, VarSym.VariableType, NewValue.Location)
            End If

            ' Create node
            Return New VariableAssignStatement(Node.Location, VarSym, NewValue)

        End Function

    End Class
End Namespace