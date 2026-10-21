Namespace HIR
    Friend Class Maker

        ' Genereate the HIR from a AST -> resolve symbols
        Public Shared Function GenerateHIR(Program As AST.ProgramNode) As HIR.ProgramNode

            Dim WorkQueue As New Lowerer.CompilationQueue 'will contain all symbols to compile
            Dim GlobalEnv As New Environment.GlobalEnvironment()
            Dim FileEnvs As Dictionary(Of AST.FileNode, Environment.FileEnvironment) = GenerateAllFileEnvs(Program, GlobalEnv)

            ' Get first symbol to compile (main function)
            Dim EntryPointSymbol As Symbol.FunctionSymbol = FileEnvs(Program.EntryFile).FindSymbol(Of Symbol.FunctionSymbol)("main", {}).FirstOrDefault()
            If EntryPointSymbol Is Nothing Then
                Throw New SyntaxException("HM01", "Cannot find the entry point (""main"" function)", Program.EntryFile.Location)
            End If
            WorkQueue.Enqueue(EntryPointSymbol) ' Add "main" to the compilation queue

            ' Create lowerer -> "visitor" for ICompilableSymbols ~ constructs (ex: functions, class, etc)
            Dim Lowerer As New Lowerer.HIRLowerer(WorkQueue, FileEnvs)
            Dim ProgramBuilder As New ProgramNode.Builder(Program.Location) 'Help symbols to push themselves into the program node
            ProgramBuilder.SetEntryPoint(EntryPointSymbol)

            ' Main compilation loop
            While WorkQueue.HasWork()
                Dim CurrentSymbolWork = WorkQueue.Dequeue()
                CurrentSymbolWork.Compiled = True
                CurrentSymbolWork.Compile(Lowerer, ProgramBuilder)
            End While

            Return ProgramBuilder.Build()

        End Function

        ' Create a Map where AST.FileNode => Environment.FileEnvironment
        Private Shared Function GenerateAllFileEnvs(Program As AST.ProgramNode, GlobalEnv As Environment.GlobalEnvironment) As Dictionary(Of AST.FileNode, Environment.FileEnvironment)
            Dim Result As New Dictionary(Of AST.FileNode, Environment.FileEnvironment)

            Dim Queue As New Queue(Of AST.FileNode)
            Queue.Enqueue(Program.EntryFile)

            While Queue.Count > 0

                Dim CurrentFile = Queue.Dequeue()
                If Not Result.ContainsKey(CurrentFile) Then
                    Result(CurrentFile) = New Environment.FileEnvironment(GlobalEnv, CurrentFile)

                    'TODO: visitor if mutlitple import types
                    For Each Imp In CurrentFile.ImportNodes
                        Queue.Enqueue(Imp.FileNode)
                    Next
                End If

            End While

            Return Result
        End Function

    End Class
End Namespace