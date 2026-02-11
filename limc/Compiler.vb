Imports System.IO
Imports System.Text

Public Module Compiler

    Private ReadOnly TEMP_C_FILE As String = Path.Combine(Path.GetTempPath(), "limc", "source.c") '%temp%/limc/source.c
    Public ReadOnly COMPILER_DIRECTORY As String = AppContext.BaseDirectory
    Public ReadOnly CFilesToInclude As New List(Of String)

    Private LimSourceFile As String
    Private Destination As String

    ' Main compiling entry point
    Public Sub Compile(LimSourceFile As String, Destination As String)
        Compiler.LimSourceFile = LimSourceFile
        Compiler.Destination = Destination

        'Compile lim source code to c source code
        CompileToC()

        'Compile c source code to executable
        If Not ONLY_COMPILE_SOURCE Then
            CompileToExe()
        End If

    End Sub

    'C
    Private Sub CompileToC()

        'Delete file if already exist
        If File.Exists(Destination) Then
            File.Delete(Destination)
        End If

        'Parse main file
        Dim MainFile As Context.SourceFile = Context.SourceFile.FromFile(LimSourceFile)

        'Getting main function from MainFile will trigger lazy-compilation
        Dim MainFunction As Lazy.Function = MainFile.FunctionRepository.RetrieveFunctions("main", {}).FirstOrDefault()
        If MainFunction Is Nothing Then
            Throw New NotMainFunctionError(MainFile)
        End If

        'Assemble all sources
        If ONLY_COMPILE_SOURCE Then
            CodeGen.AssembleFile(Destination, MainFunction.GetCompiledFunction())
        Else
            CodeGen.AssembleFile(TEMP_C_FILE, MainFunction.GetCompiledFunction())
        End If

    End Sub

    'Copy source to destination
    Private Sub CopySourceToDestination()
        File.Copy(TEMP_C_FILE, Destination, True)
    End Sub

    'Compile to executable
    Private Sub CompileToExe()

        'Create process
        Dim Process As New Process()
        Process.StartInfo.FileName = C_COMPILER_PATH
        Process.StartInfo.Arguments = GetCompilationArguments()
        Process.StartInfo.UseShellExecute = False
        Process.StartInfo.CreateNoWindow = True
        Try
            Process.Start()
        Catch ex As Exception
            Throw New BasicException("C Compiler error", $"C compiler ""{C_COMPILER_PATH}"" cannot start. Make sure this is the right path.")
        End Try
        Process.WaitForExit()
        If Not Process.ExitCode = 0 Then
            Throw New BasicException("C Compiler error", $"C compiler ""{C_COMPILER_PATH}"" returned code {Process.ExitCode}")
        End If

    End Sub

    ' Get compilation arguemnts
    Public Function GetCompilationArguments() As String

        Dim Command As New StringBuilder()
        Command.Append("-o """)
        If ONLY_COMPILE_SOURCE Then
            Command.Append(Path.GetFileNameWithoutExtension(Destination))
        Else
            Command.Append(Destination)
        End If
        Command.Append(""" """)
        If ONLY_COMPILE_SOURCE Then
            Command.Append(Destination)
        Else
            Command.Append(TEMP_C_FILE)
        End If
        Command.Append("""")
        For Each File As String In CFilesToInclude
            Command.Append(" """)
            Command.Append(File)
            Command.Append("""")
        Next

        Return Command.ToString()

    End Function

    'Get compilation command
    Public Function GetCompilationCommand() As String
        Return C_COMPILER_PATH & " " & GetCompilationArguments()
    End Function

End Module
