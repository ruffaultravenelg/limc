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

            ' Add generic types
            Dim GenericScope As New Environment.GenericEnv(FuncSymbol.DeclaringEnvironment)
            For i = 0 To FuncSymbol.PassedGeneric.Count - 1
                GenericScope.DefineGenericType(FuncSymbol.Node.GenericTypes(i).Typename, FuncSymbol.PassedGeneric(i))
                WorkQueue.Enqueue(FuncSymbol.PassedGeneric(i)) ' Compile generic types
            Next

            ' Add parameters types
            Dim ParameterScope As New Environment.BlockEnv(GenericScope)
            Dim CompiledArgs As New List(Of ArgumentNode)
            For Each arg In FuncSymbol.Node.Arguments
                Dim ArgType = TypenodeResolver.Resolve(arg.ArgumentType, GenericScope)
                Dim VarSymbol = ParameterScope.DefineVariable(arg.ArgumentName, ArgType)
                CompiledArgs.Add(New ArgumentNode(arg.Location, VarSymbol))
            Next

            ' Get return type
            Dim ReturnType As TypeSymbol = Nothing
            If FuncSymbol.Node.ReturnType IsNot Nothing Then
                ReturnType = TypenodeResolver.Resolve(FuncSymbol.Node.ReturnType, GenericScope)
            End If

            ' Compile body
            Dim Body = FuncSymbol.Node.Body.Accept(StatementCompiler, ParameterScope)

            ' Create the function
            Return New FunctionConstruct(FuncSymbol.Node.Location, FuncSymbol, CompiledArgs, ReturnType, Body)

        End Function

    End Class
End Namespace