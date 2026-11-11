Imports limc.HIR

Namespace C
    Friend Class ExpressionCompiler

        Private ValCom As IHIRExpressionVisitor(Of String)
        Private RefCom As IHIRExpressionVisitor(Of String)

        Private fs As WriterWrapper
        Private Namer As Naming.ISymbolNamer

        Public Sub New(fs As WriterWrapper, Namer As Naming.ISymbolNamer)
            Me.fs = fs
            Me.Namer = Namer

            ValCom = New ValueExprCompiler(fs, Namer, Me)
            RefCom = New LValExprCompiler(fs, Namer, Me)
        End Sub

        ' Return the value (for stack based types it could be a copie)
        Public Function CompileVal(Node As ExpressionNode) As String
            Return Node.Accept(ValCom)
        End Function

        ' Return a lvalue => direct access to symbol
        '   may throw an error
        Public Function CompileLVal(Node As ExpressionNode) As String
            Return Node.Accept(RefCom)
        End Function

        ' Returns a pointer to the value (value may be copied for stack based types)
        '   useful for self
        Public Function CompileAsPointer(Node As ExpressionNode) As String
            Dim CompiledValue = CompileVal(Node)
            If Node.ExpressionReturnType.IS_POINTER Then
                Return CompiledValue
            Else
                Dim TmpVar = Namer.GetTemp()
                fs.Startline($"{Namer.GetCompiledName(Node.ExpressionReturnType)} {TmpVar} = {CompiledValue};")
                Return $"&{TmpVar}"
            End If
        End Function

    End Class
End Namespace