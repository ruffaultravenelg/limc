Imports System.Security.AccessControl
Imports limc.HIR

Namespace C
    '
    ' Compile a HIR to a C source file
    ' Write directly to the source file, no other internal representation
    ' 
    ' Each expression is stored in a new variable. This ensure correct order, and is nessecary to allow direct write onto file stream
    '
    Friend Class HIRCompiler
        Implements IHIRVisitor

        ' Useful tools
        Private fs As WriterWrapper
        Private Namer As Naming.ISymbolNamer

        Private ExprCom As ExpressionCompiler


        ' Constructor
        Public Sub New(fs As IO.StreamWriter, Namer As Naming.ISymbolNamer)
            Me.fs = New WriterWrapper(fs)
            Me.Namer = Namer

            Me.ExprCom = New ExpressionCompiler(Me.fs, Namer)
        End Sub

        ' Fast access to CompiledName
        Private Function N(S As Symbol.Symbol) As String
            Return Namer.GetCompiledName(S)
        End Function

        ' ==== CONSTUCTS =====
        Public Sub VisitProgramNode(Program As ProgramNode) Implements IHIRVisitor.VisitProgramNode

            ' Write includes
            fs.Startline($"#include ""{RuntimeFiles.LIM_CORE_H}""")
            fs.Startline()

            ' Write all functions definitions
            For Each FunSignInfo In ConstructResolver.FindAllFunctionSignatures(Program)
                WriteFunctionSignature(FunSignInfo)
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
            fs.Startline("lim_gc_start(&argc);")
            fs.Startline($"{N(Program.EntryFunction)}();") ' TODO: better this, currently assuming entry point has no args lol
            fs.Startline("lim_gc_stop();")
            fs.Startline("return EXIT_SUCCESS;")
            fs.DecreaseIndentation()
            fs.Startline("}")
            fs.Startline()

        End Sub

        ' return_type function_name(arg_type arg_name);
        Private Sub WriteFunctionSignature(SigInfo As FunctionSignatureInfo)
            If SigInfo.ReturnType Is Nothing Then
                fs.Startline($"void {N(SigInfo.Symbol)}(")
            Else
                fs.Startline($"{N(SigInfo.ReturnType)} {N(SigInfo.Symbol)}(")
            End If
            Dim First As Boolean = True
            For Each arg In SigInfo.Arguments
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
            Dim DefaultValue As String = ExprCom.CompileVal(Node.Value)
            fs.Startline($"{N(Node.VariableSymbol.VariableType)} {N(Node.VariableSymbol)} = {DefaultValue};")
        End Sub

        Public Sub VisitVariableAssignStatement(Node As VariableAssignStatement) Implements IHIRVisitor.VisitVariableAssignStatement
            Dim NewValue As String = ExprCom.CompileVal(Node.NewValue)
            fs.Startline($"{N(Node.Variable)} = {NewValue};")
        End Sub

        Public Sub VisitProcedureCallStatement(Node As ProcedureCallStatement) Implements IHIRVisitor.VisitProcedureCallStatement

            ' Compile arguments
            Dim Arguments As New List(Of String)(Node.PassedArguments.Count + 1) ' reserve arg count + 1 for self if necessary
            For Each PassedArg In Node.PassedArguments
                Arguments.Add(ExprCom.CompileVal(PassedArg))
            Next

            ' Compile procedure expr
            Dim CallableExpr As String = CallableShortcutResolver.ResolveCallable(Node.Procedure, Arguments, fs, Namer, ExprCom)

            ' Compile
            fs.Startline($"{CallableExpr}({String.Join(", ", Arguments)});")

        End Sub

        Public Sub VisitReturnStatement(Node As ReturnStatement) Implements IHIRVisitor.VisitReturnStatement
            Dim Value As String = ExprCom.CompileVal(Node.Value)
            fs.Startline($"return {Value};")
        End Sub

    End Class
End Namespace