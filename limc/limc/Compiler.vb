Public Class Compiler

    Public Shared Sub CompileToExe(Input As String, Destination As String)

        Dim ASTProgram = AST.Maker.GenerateAST(Input)
        Dim HIRProgram = HIR.Maker.GenerateHIR(ASTProgram)

        ' Create temp C File
        Dim TempFilePath = IO.Path.GetTempFileName() & ".c"
        Dim StartInfo = C.Maker.GenerateC(HIRProgram, TempFilePath, IO.Path.GetFullPath(Destination))

        ' Compile
        Using proc As Process = Process.Start(StartInfo)
            Dim output As String = proc.StandardOutput.ReadToEnd()
            Dim err As String = proc.StandardError.ReadToEnd()

            proc.WaitForExit()

            If proc.ExitCode <> 0 Then
                Throw New RenderableException("C01", "C Compiler error", err)
            End If
        End Using

        ' Cleanup : Delete temp file
        IO.File.Delete(TempFilePath)

    End Sub

    Public Shared Sub CompileToC(Input As String, Destination As String)

        Dim ASTProgram = AST.Maker.GenerateAST(Input)
        Dim HIRProgram = HIR.Maker.GenerateHIR(ASTProgram)
        C.Maker.GenerateC(HIRProgram, Destination, "prog")

    End Sub

    Public Shared Sub CompileToToken(Input As String, Destination As String)
        Dim Fullpath As String = IO.Path.GetFullPath(Input)

        ' Read tokens
        Using sr As New IO.StreamReader(Input)
            Dim Tokens = Lexer.Tokenizer.Parse(sr, Fullpath)

            ' Write tokens
            Using fo As New OutputStream.FileOutputStream(Destination)
                Printer.TokenPrinter.PrintTokens(fo, Tokens)
            End Using
        End Using

    End Sub

    Public Shared Sub CompileToAST(Input As String, Destination As String)
        Dim Fullpath As String = IO.Path.GetFullPath(Input)

        ' Read tokens
        Using sr As New IO.StreamReader(Input)
            Dim Tokens = Lexer.Tokenizer.Parse(sr, Fullpath)

            ' Create AST for this file only (imports.filenode aren't resolved)
            Dim AbstractSyntaxTree = AST.FileParser.Parse(Tokens, Fullpath)

            ' Write AST
            Using fo As New OutputStream.FileOutputStream(Destination)
                Printer.ASTPrinter.PrintAST(fo, AbstractSyntaxTree)
            End Using
        End Using

    End Sub

    Public Shared Sub CompileToHIR(Input As String, Destination As String)
        Throw New NotImplementedException() 'TODO
    End Sub

End Class
