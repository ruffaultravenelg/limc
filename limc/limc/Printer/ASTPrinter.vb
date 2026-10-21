Imports limc.AST
Imports limc.OutputStream

Namespace Printer
    Friend Class ASTPrinter
        Implements IASTVisitor(Of ValueTuple, ValueTuple) 'value tuple because we don't need argement nor a return value

        ' Main entry point
        Public Shared Sub PrintAST(Writer As IOutputStream, Node As Node)
            Dim Visitor As New ASTPrinter(Writer)
            Node.Accept(Visitor, Nothing)
        End Sub

        ' AST Visitor
        Dim Writer As IOutputStream

        Private Sub New(Writer As IOutputStream)
            Me.Writer = Writer
        End Sub

        Private Sub Print(Node As Node)
            Node.Accept(Me, Nothing)
        End Sub
        Private Sub PrintWithComma(Nodes As IEnumerable(Of Node))
            For i = 0 To Nodes.Count - 1
                If i > 0 Then
                    Writer.Write(", ")
                End If
                Print(Nodes(i))
            Next
        End Sub

        Public Function VisitProgramNode(Node As ProgramNode, Data As ValueTuple) As ValueTuple Implements IConstructVisitor(Of ValueTuple, ValueTuple).VisitProgramNode
            Print(Node.EntryFile)
        End Function

        Public Function VisitFileNode(Node As FileNode, Data As ValueTuple) As ValueTuple Implements IConstructVisitor(Of ValueTuple, ValueTuple).VisitFileNode
            For Each im In Node.ImportNodes
                Print(im)
            Next
            For Each func In Node.Functions
                Print(func)
            Next
        End Function

        Public Function VisitFunctionConstruct(Node As FunctionConstruct, Data As ValueTuple) As ValueTuple Implements IConstructVisitor(Of ValueTuple, ValueTuple).VisitFunctionConstruct
            Writer.Write("func ")
            Writer.Write(Node.Name)
            If Node.GenericTypes.Any Then
                Writer.Write("<")
                PrintWithComma(Node.GenericTypes)
                Writer.Write(">")
            End If
            If Node.Arguments.Any Then
                Writer.Write("(")
                PrintWithComma(Node.Arguments)
                Writer.Write(")")
            End If
            If Node.ReturnType IsNot Nothing Then
                Writer.Write(":")
                Print(Node.ReturnType)
            End If
            Writer.Writeline()
            Print(Node.Body)
        End Function

        Public Function VisitArgumentNode(Node As ArgumentNode, Data As ValueTuple) As ValueTuple Implements IConstructVisitor(Of ValueTuple, ValueTuple).VisitArgumentNode
            Writer.Write(Node.ArgumentName)
            Writer.Write(":")
            Print(Node.ArgumentType)
        End Function

        Public Function VisitImportFileConstruct(Node As ImportFileConstruct, Data As ValueTuple) As ValueTuple Implements IConstructVisitor(Of ValueTuple, ValueTuple).VisitImportFileConstruct
            Writer.Write("import """)
            Writer.Writeline(Node.Filename)
            Writer.Writeline("""")
        End Function

        Public Function VisitGenericTypeParameterNode(Node As GenericTypeParameterNode, Data As ValueTuple) As ValueTuple Implements IConstructVisitor(Of ValueTuple, ValueTuple).VisitGenericTypeParameterNode
            Writer.Write(Node.Typename)
        End Function

        Public Function VisitDeclareVariableInferTypeStatement(Node As DeclareVariableInferTypeStatement, Data As ValueTuple) As ValueTuple Implements IStatementVisitor(Of ValueTuple, ValueTuple).VisitDeclareVariableInferTypeStatement
            Writer.Write("let ")
            Writer.Write(Node.VariableName)
            Writer.Write(" = ")
            Print(Node.Value)
            Writer.Writeline()
        End Function

        Public Function VisitBlockStatement(Node As BlockStatement, Data As ValueTuple) As ValueTuple Implements IStatementVisitor(Of ValueTuple, ValueTuple).VisitBlockStatement
            For Each line In Node.Statements
                Writer.Write(vbTab)
                Print(line)
            Next
        End Function

        Public Function VisitIntegerExpression(Node As IntegerExpression, Data As ValueTuple) As ValueTuple Implements IExpressionVisitor(Of ValueTuple, ValueTuple).VisitIntegerExpression
            Writer.Write(Node.Value)
        End Function

        Public Function VisitFloatExpression(Node As FloatExpression, Data As ValueTuple) As ValueTuple Implements IExpressionVisitor(Of ValueTuple, ValueTuple).VisitFloatExpression
            Writer.Write(Node.Value)
        End Function

        Public Function VisitRawStringExpression(Node As RawStringExpression, Data As ValueTuple) As ValueTuple Implements IExpressionVisitor(Of ValueTuple, ValueTuple).VisitRawStringExpression
            Writer.Write(Node.Value)
        End Function

        Public Function VisitFormattedStringExpression(Node As FormattedStringExpression, Data As ValueTuple) As ValueTuple Implements IExpressionVisitor(Of ValueTuple, ValueTuple).VisitFormattedStringExpression
            Writer.Write(Node.Value)
        End Function

        Public Function VisitBooleanExpression(Node As BooleanExpression, Data As ValueTuple) As ValueTuple Implements IExpressionVisitor(Of ValueTuple, ValueTuple).VisitBooleanExpression
            Writer.Write(Node.Value)
        End Function

        Public Function VisitGenericTypeNode(Node As GenericTypeNode, Data As ValueTuple) As ValueTuple Implements ITypeVisitor(Of ValueTuple, ValueTuple).VisitGenericTypeNode
            Writer.Write(Node.Name)
            If Node.Arguments.Any() Then
                Writer.Write("<")
                PrintWithComma(Node.Arguments)
                Writer.Write(">")
            End If
        End Function

    End Class
End Namespace