Module Program

    Sub Main(args As String())
        Try
            Dim Settings = ProgramDirective.FromArgs(args)

            ' Show help wizard
            If Settings.HelpFlag Then
                PrintHelp()
                Exit Sub
            End If

            ' Show version
            If Settings.VersionFlag Then
                PrintVersion()
                Exit Sub
            End If

            ' Compile
            Select Case Settings.Target

                Case ProgramDirective.CompilationTarget.Executable
                    Compiler.CompileToExe(Settings.InputFilename, Settings.OutputFilename)

                Case ProgramDirective.CompilationTarget.C
                    Compiler.CompileToC(Settings.InputFilename, Settings.OutputFilename)

                Case ProgramDirective.CompilationTarget.HIR
                    Compiler.CompileToHIR(Settings.InputFilename, Settings.OutputFilename)

                Case ProgramDirective.CompilationTarget.AST
                    Compiler.CompileToAST(Settings.InputFilename, Settings.OutputFilename)

                Case ProgramDirective.CompilationTarget.Token
                    Compiler.CompileToToken(Settings.InputFilename, Settings.OutputFilename)

            End Select

        Catch ex As RenderableException
            Dim ConsoleOut As New OutputStream.ConsoleOutputStream()
            ex.Render(ConsoleOut)
        End Try
    End Sub

    Sub PrintUsage()
        Console.WriteLine("Usage: {input} [output] [flags...]")
        Console.WriteLine("Use --help to show help wizard.")
    End Sub

    Sub PrintHelp()
        Console.WriteLine("Usage: {input} [output] [flags...]")
        Console.WriteLine()
        Console.WriteLine("    Input: File input path")
        Console.WriteLine()
        Console.WriteLine("    Output: File output path, optional.")
        Console.WriteLine()
        Console.WriteLine("    Flags: ")
        Console.WriteLine(vbTab & vbTab & "-e" & vbTab & "--executable" & vbTab & "Set the target result to an exectuable file")
        Console.WriteLine(vbTab & vbTab & "-c" & vbTab & "--c" & vbTab & "Set the target result to a c source file")
        Console.WriteLine(vbTab & vbTab & "-t" & vbTab & "--token" & vbTab & "Set the target result to a token file")
        Console.WriteLine(vbTab & vbTab & "-A" & vbTab & "--ast" & vbTab & "Set the target result to a AST file")
        Console.WriteLine(vbTab & vbTab & "-i" & vbTab & "--hir" & vbTab & "Set the target result to a HIR file")
        Console.WriteLine()
        Console.WriteLine(vbTab & vbTab & "-h" & vbTab & "--help" & vbTab & "Set the target result to a HIR file")
    End Sub

    Sub PrintVersion()
        Console.WriteLine("Lim compiler (limc) version 4.0.0")
    End Sub

End Module
