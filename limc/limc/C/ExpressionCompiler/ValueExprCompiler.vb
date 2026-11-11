Imports System.Globalization
Imports limc.HIR

Namespace C
    '
    ' lvalue compiler
    ' => compile reference expression
    ' mainly used on the left side of assignation 
    '
    Friend Class ValueExprCompiler
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
            Return Node.Value.ToString()
        End Function

        Public Function VisitFloatExpression(Node As FloatExpression) As String Implements IHIRExpressionVisitor(Of String).VisitFloatExpression
            Return Node.Value.ToString("F", CultureInfo.InvariantCulture)
        End Function

        Public Function VisitBoolExpression(Node As BoolExpression) As String Implements IHIRExpressionVisitor(Of String).VisitBoolExpression
            Return If(Node.Value, "true", "else")
        End Function

        Public Function VisitRawStrExpression(Node As RawStrExpression) As String Implements IHIRExpressionVisitor(Of String).VisitRawStrExpression
            Return $"lim_literal2str(""{Node.Value}"")"
        End Function

        Public Function VisitFormattedStrExpression(Node As FormattedStrExpression) As String Implements IHIRExpressionVisitor(Of String).VisitFormattedStrExpression
            Throw New NotImplementedException()
        End Function

        Public Function VisitVariableExpression(Node As VariableExpression) As String Implements IHIRExpressionVisitor(Of String).VisitVariableExpression
            Return N(Node.VarSymbol)
        End Function

        Public Function VisitProcedureCallExpression(Node As ProcedureCallExpression) As String Implements IHIRExpressionVisitor(Of String).VisitProcedureCallExpression

            ' Compile arguments
            Dim Arguments As New List(Of String)(Node.PassedArguments.Count + 1) ' reserve arg count + 1 for self if necessary
            For Each PassedArg In Node.PassedArguments
                Arguments.Add(ExprCom.CompileVal(PassedArg))
            Next

            ' Compile procedure expr
            Dim CallableExpr As String = CallableShortcutResolver.ResolveCallable(Node.Procedure, Arguments, fs, namer, ExprCom)

            ' Compile
            Return $"{CallableExpr}({String.Join(", ", Arguments)})"

        End Function

        Public Function VisitFunctionReferenceExpression(Node As FunctionReferenceExpression) As String Implements IHIRExpressionVisitor(Of String).VisitFunctionReferenceExpression
            Throw New NotImplementedException()
        End Function

        Public Function VisitMethodReferenceExpression(Node As ObjMethodReferenceExpression) As String Implements IHIRExpressionVisitor(Of String).VisitMethodReferenceExpression
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace