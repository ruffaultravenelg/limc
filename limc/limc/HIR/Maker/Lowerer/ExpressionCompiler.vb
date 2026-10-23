Imports System.Text.RegularExpressions
Imports limc.Environment
Imports limc.Symbol

Namespace HIR.Lowerer
    Friend Class ExpressionCompiler
        Implements AST.IExpressionVisitor(Of Env, ExpressionNode)

        Private WorkQueue As CompilationQueue

        Public Sub New(WorkQueue As CompilationQueue)
            Me.WorkQueue = WorkQueue
        End Sub

        Public Function VisitIntegerExpression(Node As AST.IntegerExpression, Ctx As Env) As ExpressionNode Implements AST.IExpressionVisitor(Of Env, ExpressionNode).VisitIntegerExpression
            Return New IntExpression(Node.Location, Node.Value)
        End Function

        Public Function VisitFloatExpression(Node As AST.FloatExpression, Ctx As Env) As ExpressionNode Implements AST.IExpressionVisitor(Of Env, ExpressionNode).VisitFloatExpression
            Return New FloatExpression(Node.Location, Node.Value)
        End Function

        Public Function VisitBooleanExpression(Node As AST.BooleanExpression, Ctx As Env) As ExpressionNode Implements AST.IExpressionVisitor(Of Env, ExpressionNode).VisitBooleanExpression
            Return New BoolExpression(Node.Location, Node.Value)
        End Function

        Public Function VisitRawStringExpression(Node As AST.RawStringExpression, Ctx As Env) As ExpressionNode Implements AST.IExpressionVisitor(Of Env, ExpressionNode).VisitRawStringExpression
            Return New RawStrExpression(Node.Location, Node.Value)
        End Function

        Public Function VisitFormattedStringExpression(Node As AST.FormattedStringExpression, Ctx As Env) As ExpressionNode Implements AST.IExpressionVisitor(Of Env, ExpressionNode).VisitFormattedStringExpression
            Return New FormattedStrExpression(Node.Location, Node.Value)
        End Function

        Public Function VisitProcedureCallExpression(Node As AST.ProcedureCallExpression, Ctx As Env) As ExpressionNode Implements AST.IExpressionVisitor(Of Env, ExpressionNode).VisitProcedureCallExpression

            ' Compiled arguments
            Dim CompiledArguments As New List(Of ExpressionNode)
            For Each arg In Node.PassedArguments
                CompiledArguments.Add(arg.Accept(Me, Ctx)) 'each argument will compile it's types
            Next

            ' Child may need passed arguments types to resolve which function to choose (due to function overloading)
            ' this env help to pass them this data
            Dim HintEnv As New CallHintEnv(Ctx, CompiledArguments.Select(Function(arg) arg.ExpressionReturnType))

            ' Compile procedure with hint
            Dim CompiledProcedure As ExpressionNode = Node.Procedure.Accept(Me, HintEnv)
            If TypeOf CompiledProcedure.ExpressionReturnType IsNot FuncTypeSymbol Then
                Throw New TypeMismatchException("HC01", CompiledProcedure.ExpressionReturnType, "whereas a ""func"" type was expected.", CompiledProcedure.Location)
            End If

            ' Create call
            Return New ProcedureCallExpression(Node.Location, CompiledProcedure, CompiledArguments)

        End Function

        Public Function VisitElementExpression(Node As AST.ElementExpression, Ctx As Env) As ExpressionNode Implements AST.IExpressionVisitor(Of Env, ExpressionNode).VisitElementExpression

            Dim BestMatch As SymbolMatch

            If CallHintEnv.IsHint(Ctx) Then
                'we need to find a specific function -> use procedure overloading resolver

                Dim matches = Ctx.FindSymbol(Node.ElementName, {})
                BestMatch = ProcedureOverloadingResolver.SelectBestMatch(matches, CallHintEnv.GetHintedTypes(Ctx))
                If BestMatch Is Nothing Then
                    Throw New SyntaxException("HC02", "No matching function", Node.Location)
                End If
            Else
                'just get the first symbol
                BestMatch = Ctx.FindSymbol(Node.ElementName, {}).First()
            End If

            ' Compile match to node
            Select Case True

                Case BestMatch.IsType(Of VariableSymbol)
                    Return New VariableExpression(Node.Location, BestMatch.VARIABLE_SYMBOL)

                Case BestMatch.IsType(Of FunctionSymbol)
                    WorkQueue.Enqueue(BestMatch.FUNCTION_SYMBOL) 'compile called function
                    WorkQueue.Enqueue(Primitives.Func(BestMatch.FUNCTION_SYMBOL.Arguments.Select(Function(arg) arg.VariableType), BestMatch.FUNCTION_SYMBOL.ReturnType)) 'get the func type of the function symbol and compile it
                    Return New FunctionReferenceExpression(Node.Location, BestMatch.FUNCTION_SYMBOL)

                Case Else
                    Throw New NotImplementedException()

            End Select

        End Function

        Public Function VisitGenericElementExpression(Node As AST.GenericElementExpression, Ctx As Env) As ExpressionNode Implements AST.IExpressionVisitor(Of Env, ExpressionNode).VisitGenericElementExpression

            Dim PassedGenericTypes As New List(Of TypeSymbol)
            For Each G In Node.PassedGenerics
                Dim T = TypenodeResolver.Resolve(G, Ctx)
                WorkQueue.Enqueue(T)
                PassedGenericTypes.Add(T)
            Next

            Dim BestMatch As SymbolMatch

            If CallHintEnv.IsHint(Ctx) Then
                'we need to find a specific function -> use procedure overloading resolver

                Dim matches = Ctx.FindSymbol(Node.ElementName, PassedGenericTypes)
                BestMatch = ProcedureOverloadingResolver.SelectBestMatch(matches, CallHintEnv.GetHintedTypes(Ctx))
                If BestMatch Is Nothing Then
                    Throw New SyntaxException("HC02", "No matching function", Node.Location)
                End If
            Else
                'just get the first symbol
                BestMatch = Ctx.FindSymbol(Node.ElementName, PassedGenericTypes).First()
            End If

            ' Compile match to node
            Select Case True

                Case BestMatch.IsType(Of VariableSymbol)
                    Return New VariableExpression(Node.Location, BestMatch.VARIABLE_SYMBOL)

                Case BestMatch.IsType(Of FunctionSymbol)
                    Return New FunctionReferenceExpression(Node.Location, BestMatch.FUNCTION_SYMBOL)

                Case Else
                    Throw New NotImplementedException()

            End Select

        End Function

    End Class
End Namespace