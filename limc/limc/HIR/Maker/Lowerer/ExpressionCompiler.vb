Imports limc.Environment

Namespace HIR.Lowerer
    Friend Class ExpressionCompiler
        Implements AST.IExpressionVisitor(Of BlockEnv, ExpressionNode)

        Private WorkQueue As CompilationQueue

        Public Sub New(WorkQueue As CompilationQueue)
            Me.WorkQueue = WorkQueue
        End Sub

        Public Function VisitIntegerExpression(Node As AST.IntegerExpression, Ctx As BlockEnv) As ExpressionNode Implements AST.IExpressionVisitor(Of BlockEnv, ExpressionNode).VisitIntegerExpression
            Return New IntNode(Node.Location, Node.Value)
        End Function

        Public Function VisitFloatExpression(Node As AST.FloatExpression, Ctx As BlockEnv) As ExpressionNode Implements AST.IExpressionVisitor(Of BlockEnv, ExpressionNode).VisitFloatExpression
            Throw New NotImplementedException()
        End Function

        Public Function VisitRawStringExpression(Node As AST.RawStringExpression, Ctx As BlockEnv) As ExpressionNode Implements AST.IExpressionVisitor(Of BlockEnv, ExpressionNode).VisitRawStringExpression
            Throw New NotImplementedException()
        End Function

        Public Function VisitFormattedStringExpression(Node As AST.FormattedStringExpression, Ctx As BlockEnv) As ExpressionNode Implements AST.IExpressionVisitor(Of BlockEnv, ExpressionNode).VisitFormattedStringExpression
            Throw New NotImplementedException()
        End Function

        Public Function VisitBooleanExpression(Node As AST.BooleanExpression, Ctx As BlockEnv) As ExpressionNode Implements AST.IExpressionVisitor(Of BlockEnv, ExpressionNode).VisitBooleanExpression
            Throw New NotImplementedException()
        End Function

    End Class
End Namespace