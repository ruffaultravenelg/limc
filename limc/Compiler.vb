Imports System.IO
Imports System.Text

Public Module Compiler

    Public ReadOnly TEMP_C_FILE As String = Path.Combine(Path.GetTempPath(), "limc", "source.c") '%temp%/limc/source.c
    Public ReadOnly COMPILER_DIRECTORY As String = AppContext.BaseDirectory
    Public ReadOnly CFilesToInclude As New List(Of String)

    ' Main compiling entry point
    Public Sub Compile(SourceFile As String, Destination As String)

        'Compile lim source code to c source code
        CompileToC(SourceFile, TEMP_C_FILE)

        'Compile c source code to executable
        'CompileToExe(TEMP_C_FILE, Destination)

    End Sub

    'C
    Private Sub CompileToC(SourceFilepath As String, Destination As String)

        'Delete file if already exist
        If File.Exists(Destination) Then
            File.Delete(Destination)
        End If

        'Parse main file
        Dim MainFile As Context.SourceFile = Context.SourceFile.FromFile(SourceFilepath)

        'Getting main function from MainFile will trigger lazy-compilation
        Dim MainFunction As Lazy.Function = MainFile.FunctionRepository.RetrieveFunction("main")
        If MainFunction Is Nothing Then
            Throw New NotMainFunctionError(MainFile)
        End If

        'Assemble all sources
        CodeGen.AssembleFile(Destination, MainFunction.FuncScope.GeneratedFunction)

    End Sub

    'Compile to executable
    Private Sub CompileToExe(SourceC As String, Destination As String)

        Dim Command As String = GetCompilationCommand(SourceC, Destination)

        'Create process
        Dim Process As New Process()
        Process.StartInfo.FileName = "cmd.exe"
        Process.StartInfo.Arguments = "/c " & Command
        Process.StartInfo.RedirectStandardOutput = True
        Process.StartInfo.RedirectStandardError = True
        Process.StartInfo.UseShellExecute = False
        Process.StartInfo.CreateNoWindow = True
        Process.Start()
        Process.WaitForExit()
        Dim Output As String = Process.StandardOutput.ReadToEnd()
        Dim [Error] As String = Process.StandardError.ReadToEnd()
        If Not Process.ExitCode = 0 Then
            Throw New BasicException("GCC Error", Output & vbCrLf & [Error])
        End If

    End Sub

    'Get compilation command
    Public Function GetCompilationCommand(Source As String, Destination As String) As String

        Dim Command As New StringBuilder()
        Command.Append("gcc")
        Command.Append(" -o """)
        Command.Append(Destination)
        Command.Append(""" """)
        Command.Append(Source)
        Command.Append("""")
        For Each File As String In CFilesToInclude
            Command.Append(" """)
            Command.Append(File)
            Command.Append("""")
        Next

        Return Command.ToString()

    End Function

End Module
