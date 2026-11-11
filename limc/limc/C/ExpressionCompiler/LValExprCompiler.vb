Imports limc.HIR

Namespace C
    '
    ' lvalue compiler
    ' => compile reference expression
    ' mainly used on the left side of assignation 
    '
    Friend Class LValExprCompiler
        Implements IHIRExpressionVisitor(Of String)

        Private fs As WriterWrapper
        Private namer As Naming.ISymbolNamer
        Private ExprCom As ExpressionCompiler

        Public Sub New(fs As WriterWrapper, Namer As Naming.ISymbolNamer, ExprCom As ExpressionCompiler)
            Me.fs = fs
            Me.namer = Namer
            Me.ExprCom = ExprCom
        End Sub

        ' Fast access to CompiledName
        Private Function N(S As Symbol.Symbol) As String
            Return namer.GetCompiledName(S)
        End Function

        Public Function VisitIntExpression(Node As IntExpression) As String Implements IHIRExpressionVisitor(Of String).VisitIntExpression
            Throw New ExpressionIsntLvalException("CEL01", Node.Location)
        End Function

        Public Function VisitFloatExpression(Node As FloatExpression) As String Implements IHIRExpressionVisitor(Of String).VisitFloatExpression
            Throw New ExpressionIsntLvalException("CEL01", Node.Location)
        End Function

        Public Function VisitBoolExpression(Node As BoolExpression) As String Implements IHIRExpressionVisitor(Of String).VisitBoolExpression
            Throw New ExpressionIsntLvalException("CEL01", Node.Location)
        End Function

        Public Function VisitRawStrExpression(Node As RawStrExpression) As String Implements IHIRExpressionVisitor(Of String).VisitRawStrExpression
            Throw New ExpressionIsntLvalException("CEL01", Node.Location)
        End Function

        Public Function VisitFormattedStrExpression(Node As FormattedStrExpression) As String Implements IHIRExpressionVisitor(Of String).VisitFormattedStrExpression
            Throw New ExpressionIsntLvalException("CEL01", Node.Location)
        End Function

        Public Function VisitProcedureCallExpression(Node As ProcedureCallExpression) As String Implements IHIRExpressionVisitor(Of String).VisitProcedureCallExpression
            If Not Node.ExpressionReturnType.IS_POINTER Then
                Throw New ExpressionIsntLvalException("CEL01", Node.Location)
            Else
                Return ExprCom.CompileVal(Node) ' the returned value is a pointer
            End If
        End Function

        Public Function VisitVariableExpression(Node As VariableExpression) As String Implements IHIRExpressionVisitor(Of String).VisitVariableExpression
            Return N(Node.VarSymbol)
        End Function

        Public Function VisitFunctionReferenceExpression(Node As FunctionReferenceExpression) As String Implements IHIRExpressionVisitor(Of String).VisitFunctionReferenceExpression
            Throw New SyntaxException("CEL02", "A function cannot be used as a lvalue (not assignable).", Node.Location)
        End Function

        Public Function VisitMethodReferenceExpression(Node As ObjMethodReferenceExpression) As String Implements IHIRExpressionVisitor(Of String).VisitMethodReferenceExpression
            Throw New SyntaxException("CEL03", "A method cannot be used as a lvalue (not assignable).", Node.Location)
        End Function
    End Class
End Namespace