Imports limc.HIR
Imports limc.Symbol

Namespace C
    Friend Class HIRCompiler
        Implements HIR.IHIRVisitor

        ' Useful tools
        Private fs As WriterWrapper
        Private Namer As Naming.ISymbolNamer

        ' Constructor
        Public Sub New(fs As IO.StreamWriter, Namer As Naming.ISymbolNamer)
            Me.fs = New WriterWrapper(fs)
            Me.Namer = Namer
        End Sub


        ' Fast access to CompiledName
        Private Function N(S As Symbol.Symbol) As String
            Return Namer.GetCompiledName(S)
        End Function

        ' Macro to help writing ", " separated elements
        Private Sub WriteListWithComa(Elements As IEnumerable(Of HIR.Node))
            Dim First As Boolean = True
            For Each Elm In Elements
                If Not First Then
                    fs.Write(", ")
                End If
                Elm.Accept(Me)
                First = False
            Next
        End Sub

        ' ==== CONSTUCTS =====
        Public Sub VisitProgramNode(Program As ProgramNode) Implements IHIRVisitor.VisitProgramNode

            ' Write includes
            For Each core_header In RuntimeFiles.GetHeaders()
                fs.Startline($"#include ""{core_header}""")
            Next
            fs.Startline()

            ' Write function definitions
            For Each fun In Program.Functions
                WriteFunctionSignature(fun)
            Next
            fs.Startline()

            ' Write function bodies
            For Each fun In Program.Functions
                fun.Accept(Me)
                fs.Startline()
            Next

            ' Write main function
            fs.Startline("int main(int argc, char* argv[]) {")
            fs.IncreaseIndentation()
            fs.Startline($"{N(Program.EntryFunction)}();") ' TODO: better this, currently assuming entry point has no args lol
            fs.DecreaseIndentation()
            fs.Startline("}")
            fs.Startline()

        End Sub

        ' return_type function_name(arg_type arg_name);
        Private Sub WriteFunctionSignature(Func As FunctionConstruct)
            If Func.Symbol.ReturnType Is Nothing Then
                fs.Startline($"void {N(Func.Symbol)}(")
            Else
                fs.Startline($"{N(Func.Symbol.ReturnType)} {N(Func.Symbol)}(")
            End If
            Dim First As Boolean = True
            For Each arg In Func.Symbol.Arguments
                If Not First Then
                    fs.Write(", ")
                End If
                fs.Write($"{N(arg.VariableType)} {N(arg)}")
                First = False
            Next
            fs.Write(");")
        End Sub

        Public Sub VisitFunctionConstruct(Func As FunctionConstruct) Implements IHIRVisitor.VisitFunctionConstruct

            ' Signature
            If Func.Symbol.ReturnType Is Nothing Then
                fs.Startline($"void {N(Func.Symbol)}(")
            Else
                fs.Startline($"{N(Func.Symbol.ReturnType)} {N(Func.Symbol)}(")
            End If
            Dim First As Boolean = True
            For Each arg In Func.Symbol.Arguments
                If Not First Then
                    fs.Write(", ")
                End If
                fs.Write($"{N(arg.VariableType)} {N(arg)}")
                First = False
            Next
            fs.Write("){")

            ' Body
            Func.Body.Accept(Me)

            ' End
            fs.Startline("}")

        End Sub

        Public Sub VisitArgumentNode(Node As ArgumentNode) Implements IHIRVisitor.VisitArgumentNode
            fs.Write($"{N(Node.Symbol.VariableType)} {N(Node.Symbol)}")
        End Sub

        ' ==== STATEMENTS ====
        Public Sub VisitBlockStatmement(Node As BlockStatement) Implements IHIRVisitor.VisitBlockStatmement
            fs.IncreaseIndentation()
            For Each Statement In Node.Statements
                Statement.Accept(Me)
            Next
            fs.DecreaseIndentation()
        End Sub

        Public Sub VisitDeclareVariableStatement(Node As DeclareVariableStatement) Implements IHIRVisitor.VisitDeclareVariableStatement
            fs.Startline($"{N(Node.VariableSymbol.VariableType)} {N(Node.VariableSymbol)} = ")
            Node.Value.Accept(Me)
            fs.Write(";")
        End Sub

        ' ==== EXPRESSIONS ====
        Public Sub VisitIntExpression(Node As IntExpression) Implements IHIRVisitor.VisitIntExpression
            fs.Write("((int)" & Node.Value.ToString() & ")")
        End Sub

        Public Sub VisitFloatExpression(Node As FloatExpression) Implements IHIRVisitor.VisitFloatExpression
            fs.Write("((double)" & Node.Value.ToString() & ")")
        End Sub

        Public Sub VisitBoolExpression(Node As BoolExpression) Implements IHIRVisitor.VisitBoolExpression
            fs.Write(If(Node.Value, "true", "false"))
        End Sub

        Public Sub VisitRawStrExpression(Node As RawStrExpression) Implements IHIRVisitor.VisitRawStrExpression
            Throw New NotImplementedException()
        End Sub

        Public Sub VisitFormattedStrExpression(Node As FormattedStrExpression) Implements IHIRVisitor.VisitFormattedStrExpression
            Throw New NotImplementedException()
        End Sub

        Public Sub VisitProcedureCallExpression(Node As ProcedureCallExpression) Implements IHIRVisitor.VisitProcedureCallExpression

            ' Write function reference
            If TypeOf Node.Procedure Is FunctionReferenceExpression Then
                'short-circut => name of the function instead of compiling the function reference pointer
                fs.Write(N(DirectCast(Node.Procedure, FunctionReferenceExpression).FuncSymbol))
            Else
                Node.Procedure.Accept(Me)
            End If

            ' Compile arguments
            fs.Write("(")
            WriteListWithComa(Node.PassedArguments)
            fs.Write(")")

        End Sub

        Public Sub VisitVariableExpression(Node As VariableExpression) Implements IHIRVisitor.VisitVariableExpression
            fs.Write(N(Node.VarSymbol))
        End Sub

        Public Sub VisitFunctionReferenceExpression(Node As FunctionReferenceExpression) Implements IHIRVisitor.VisitFunctionReferenceExpression
            Throw New NotImplementedException()
        End Sub

    End Class
End Namespace