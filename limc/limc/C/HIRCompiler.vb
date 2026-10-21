Imports limc.HIR

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
            If Func.ReturnType Is Nothing Then
                fs.Startline($"void {N(Func.Symbol)}(")
            Else
                fs.Startline($"{N(Func.ReturnType)} {N(Func.Symbol)}(")
            End If
            For Each arg In Func.Arguments
                arg.Accept(Me)
            Next
            fs.Write(");")
        End Sub

        Public Sub VisitFunctionConstruct(Func As FunctionConstruct) Implements IHIRVisitor.VisitFunctionConstruct

            ' Signature
            If Func.ReturnType Is Nothing Then
                fs.Startline($"void {N(Func.Symbol)}(")
            Else
                fs.Startline($"{N(Func.ReturnType)} {N(Func.Symbol)}(")
            End If
            For Each arg In Func.Arguments
                arg.Accept(Me)
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
        Public Sub VisitIntNode(Node As IntNode) Implements IHIRVisitor.VisitIntNode
            fs.Write("((int)" & Node.Value.ToString() & ")")
        End Sub


    End Class
End Namespace