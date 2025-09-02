Imports System.IO

Public Module Compiler

    Private ReadOnly TEMP_C_FILE As String = Path.Combine(Path.GetTempPath(), "limc", "source.c") '%temp%/limc/source.c

    ' Main compiling entry point
    Public Sub Compile(SourceFile As String, Destination As String)

        'Compile lim source code to c source code
        CompileToC(SourceFile, TEMP_C_FILE)

        'Compile c source code to executable
        'TODO

    End Sub

    'C
    Public Sub CompileToC(SourceFilepath As String, Destination As String)

        'Delete file if already exist
        If File.Exists(Destination) Then
            File.Delete(Destination)
        End If

        'Parse main file
        Dim MainFile As SourceFile = SourceFile.FromFile(SourceFilepath)

        'Getting main function from MainFile will trigger lazy-compilation
        Dim MainFunction As Lazy.Function = MainFile.FunctionRepository.FindProcedure("main", {})
        If MainFunction Is Nothing Then
            Throw New NotMainFunctionError(MainFile)
        End If

        'Assemble all sources
        CodeGen.AssembleFile(Destination, MainFunction.CompiledFunctionName)

    End Sub

End Module
