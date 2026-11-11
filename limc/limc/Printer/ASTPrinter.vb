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
        Private Writer As IOutputStream
        Private IndentationLevel As Integer

        Private Sub New(Writer As IOutputStream)
            Me.Writer = Writer
        End Sub

        ' ==== UTIL PRINT FUNCTIONS ====
        Private Sub Write(Text As String)
            Writer.Write(Text)
        End Sub
        Private Sub StartLine(Optional Text As String = "")
            Writer.Writeline()
            Writer.Write(StrDup(IndentationLevel, vbTab))
            Writer.Write(Text)
        End Sub
        Private Sub IncreaseIdent()
            IndentationLevel += 1
        End Sub
        Private Sub DecreaseIdent()
            IndentationLevel -= 1
            If IndentationLevel < 0 Then
                Throw New InternalException("cannot write sub 0 indentation")
            End If
        End Sub
        Private Sub Write(Node As Node)
            Node.Accept(Me, Nothing)
        End Sub
        Private Sub WriteList(Nodes As IEnumerable(Of Node))
            For i = 0 To Nodes.Count - 1
                If i > 0 Then
                    Writer.Write(", ")
                End If
                Write(Nodes(i))
            Next
        End Sub
        Private Sub WriteOptList(Left As String, Nodes As IEnumerable(Of Node), Right As String)
            If Nodes.Any Then
                Write(Left)
                WriteList(Nodes)
                Write(Right)
            End If
        End Sub

        ' ==== VISITORS ====
        Public Function VisitProgramNode(Node As ProgramNode, Data As ValueTuple) As ValueTuple Implements IConstructVisitor(Of ValueTuple, ValueTuple).VisitProgramNode
            Write(Node.EntryFile)
        End Function

        Public Function VisitFileNode(Node As FileNode, Data As ValueTuple) As ValueTuple Implements IConstructVisitor(Of ValueTuple, ValueTuple).VisitFileNode
            For Each im In Node.ImportNodes
                Write(im)
            Next
            StartLine()

            For Each func In Node.Functions
                Write(func)
            Next
            StartLine()

            For Each Cl In Node.Classes
                Write(Cl)
            Next
            StartLine()
        End Function

        Public Function VisitFunctionConstruct(Node As FunctionConstruct, Data As ValueTuple) As ValueTuple Implements IConstructVisitor(Of ValueTuple, ValueTuple).VisitFunctionConstruct
            StartLine("func ")
            Write(Node.Name)
            WriteOptList("<", Node.GenericTypes, ">")
            WriteOptList("(", Node.Arguments, ")")
            If Node.ReturnType IsNot Nothing Then
                Write(":")
                Write(Node.ReturnType)
            End If
            Write("{")
            Write(Node.Body)
            StartLine("}")
        End Function

        Public Function VisitArgumentNode(Node As ArgumentNode, Data As ValueTuple) As ValueTuple Implements IConstructVisitor(Of ValueTuple, ValueTuple).VisitArgumentNode
            Writer.Write(Node.ArgumentName)
            Writer.Write(":")
            Write(Node.ArgumentType)
        End Function

        Public Function VisitImportFileConstruct(Node As ImportFileConstruct, Data As ValueTuple) As ValueTuple Implements IConstructVisitor(Of ValueTuple, ValueTuple).VisitImportFileConstruct
            StartLine($"import ""{Node.Filename}""")
        End Function

        Public Function VisitGenericTypeParameterNode(Node As GenericTypeParameterNode, Data As ValueTuple) As ValueTuple Implements IConstructVisitor(Of ValueTuple, ValueTuple).VisitGenericTypeParameterNode
            Write(Node.Typename)
        End Function

        Public Function VisitDeclareVariableInferTypeStatement(Node As DeclareVariableInferTypeStatement, Data As ValueTuple) As ValueTuple Implements IStatementVisitor(Of ValueTuple, ValueTuple).VisitDeclareVariableInferTypeStatement
            StartLine($"let {Node.VariableName} = ")
            Write(Node.Value)
            Write(";")
        End Function

        Public Function VisitBlockStatement(Node As BlockStatement, Data As ValueTuple) As ValueTuple Implements IStatementVisitor(Of ValueTuple, ValueTuple).VisitBlockStatement
            IncreaseIdent()
            For Each line In Node.Statements
                Write(line)
            Next
            DecreaseIdent()
        End Function

        Public Function VisitIntegerExpression(Node As IntegerExpression, Data As ValueTuple) As ValueTuple Implements IExpressionVisitor(Of ValueTuple, ValueTuple).VisitIntegerExpression
            Write(Node.Value)
        End Function

        Public Function VisitFloatExpression(Node As FloatExpression, Data As ValueTuple) As ValueTuple Implements IExpressionVisitor(Of ValueTuple, ValueTuple).VisitFloatExpression
            Write(Node.Value)
        End Function

        Public Function VisitRawStringExpression(Node As RawStringExpression, Data As ValueTuple) As ValueTuple Implements IExpressionVisitor(Of ValueTuple, ValueTuple).VisitRawStringExpression
            Write(Node.Value)
        End Function

        Public Function VisitFormattedStringExpression(Node As FormattedStringExpression, Data As ValueTuple) As ValueTuple Implements IExpressionVisitor(Of ValueTuple, ValueTuple).VisitFormattedStringExpression
            Write(Node.Value)
        End Function

        Public Function VisitBooleanExpression(Node As BooleanExpression, Data As ValueTuple) As ValueTuple Implements IExpressionVisitor(Of ValueTuple, ValueTuple).VisitBooleanExpression
            Write(Node.Value)
        End Function

        Public Function VisitGenericTypeNode(Node As GenericTypeNode, Data As ValueTuple) As ValueTuple Implements ITypeVisitor(Of ValueTuple, ValueTuple).VisitGenericTypeNode
            Write(Node.Name)
            WriteOptList("<", Node.Arguments, ">")
        End Function

        Public Function VisitProcedureCallExpression(Node As ProcedureCallExpression, Data As ValueTuple) As ValueTuple Implements IExpressionVisitor(Of ValueTuple, ValueTuple).VisitProcedureCallExpression
            Write(Node.Procedure)
            WriteOptList("(", Node.PassedArguments, ")")
        End Function

        Public Function VisitElementExpression(Node As ElementExpression, Data As ValueTuple) As ValueTuple Implements IExpressionVisitor(Of ValueTuple, ValueTuple).VisitElementExpression
            Write(Node.ElementName)
        End Function

        Public Function VisitGenericElementExpression(Node As GenericElementExpression, Data As ValueTuple) As ValueTuple Implements IExpressionVisitor(Of ValueTuple, ValueTuple).VisitGenericElementExpression
            Write(Node.ElementName)
            WriteOptList("<", Node.PassedGenerics, ">")
        End Function

        Public Function VisitProcedureCallStatement(Node As ProcedureCallStatement, Data As ValueTuple) As ValueTuple Implements IStatementVisitor(Of ValueTuple, ValueTuple).VisitProcedureCallStatement
            StartLine()
            Write(Node.Procedure)
            Write("(")
            WriteList(Node.PassedArguments)
            Write(");")
        End Function

        Public Function VisitVariableAssignStatement(Node As VariableAssignStatement, Data As ValueTuple) As ValueTuple Implements IStatementVisitor(Of ValueTuple, ValueTuple).VisitVariableAssignStatement
            StartLine()
            Write(Node.Variable)
            Write(" = ")
            Write(Node.NewValue)
            Write(";")
        End Function

        Public Function VisitClassConstruct(Node As ClassConstruct, Data As ValueTuple) As ValueTuple Implements IConstructVisitor(Of ValueTuple, ValueTuple).VisitClassConstruct

            StartLine("class ")
            Write(Node.ClassName)
            WriteOptList("<", Node.GenericTypes, ">")
            Write("{")
            IncreaseIdent()

            For Each prop In Node.Properties
                Write(prop)
            Next

            For Each cons In Node.Constructors
                Write(cons)
            Next

            DecreaseIdent()
            StartLine("}")

        End Function

        Public Function VisitPropertieNode(Node As PropertieNode, Data As ValueTuple) As ValueTuple Implements IConstructVisitor(Of ValueTuple, ValueTuple).VisitPropertieNode
            StartLine("let ")
            Write(Node.PropertieName)
            Write(" = ")
            Write(Node.PropertieDefaultValue)
            Write(";")
        End Function

        Public Function VisitConstructorConstruct(Node As ConstructorConstruct, Data As ValueTuple) As ValueTuple Implements IConstructVisitor(Of ValueTuple, ValueTuple).VisitConstructorConstruct
            StartLine("new")
            WriteOptList("(", Node.Arguments, ")")
            Write("{")
            Write(Node.Body)
            StartLine("}")
        End Function

        Public Function VisitMemberExpression(Node As MemberExpression, Data As ValueTuple) As ValueTuple Implements IExpressionVisitor(Of ValueTuple, ValueTuple).VisitMemberExpression
            Write(Node.Element)
            Write(".")
            Write(Node.MemberName)
            WriteOptList("<", Node.PassedGenerics, ">")
        End Function

        Public Function VisitReturnStatement(Node As ReturnStatement, Data As ValueTuple) As ValueTuple Implements IStatementVisitor(Of ValueTuple, ValueTuple).VisitReturnStatement
            StartLine("return ")
            Write(Node.Value)
            Write(";")
        End Function

    End Class
End Namespace