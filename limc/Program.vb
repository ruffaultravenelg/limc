Module Program

    Private Const LIMC_VERSION As String = "1.0.0"

    Public Property VERBOSE As Boolean = False
    Public Property INTEGRATE_DEBUG As Boolean = True
    Public Property ONLY_COMPILE_SOURCE As Boolean = False
    Public Property C_COMPILER_PATH As String = "gcc"
    Sub Main(args As String())

        Dim PositionalArgs As New List(Of String)()

        For i As Integer = 0 To args.Length - 1
            Dim arg As String = args(i)

            Select Case arg.ToLower()
                Case "-h", "--help"
                    Help()
                    Exit Sub
                Case "-v", "--version"
                    Version()
                    Exit Sub
                Case "-vb", "--verbose"
                    VERBOSE = True
                Case "-r", "--release"
                    INTEGRATE_DEBUG = False
                Case "-s", "--source"
                    ONLY_COMPILE_SOURCE = True
                Case "-cc"
                    If i + 1 < args.Length Then
                        C_COMPILER_PATH = args(i + 1)
                        i += 1
                    Else
                        Console.WriteLine("Error: Missing path for -cc argument.")
                        Exit Sub
                    End If
                Case Else
                    PositionalArgs.Add(arg)
            End Select
        Next

        Dim SourcePath As String = Nothing
        Dim DestinationPath As String = Nothing
        If PositionalArgs.Count = 2 Then
            SourcePath = PositionalArgs(0)
            DestinationPath = PositionalArgs(1)
        Else
            Console.WriteLine("Error: Both input and output must be specified.")
            Console.WriteLine("Usage: limc [input] [output] [flags...]")
            Exit Sub
        End If

        'Start compiling
        Compile(SourcePath, DestinationPath)
        Try
            Compile(SourcePath, DestinationPath)
        Catch ex As RenderableException
            ex.Render()
#If Not DEBUG Then
        Catch ex As Exception
            Dim InternalExcepetion As New BasicException("Internal error", "A unexpected error was thrown by the compiler.")
            InternalExcepetion.Render()
#End If
        End Try
        Console.WriteLine("end")

    End Sub

    Private Sub Version()
        Console.WriteLine("Lim compiler (limc)")
        Console.WriteLine($"version {LIMC_VERSION}")
    End Sub

    Private Sub Help()

        Console.WriteLine("Lim compiler (limc)")
        Console.WriteLine("Usage: limc [input] [output] [flags...]")
        Console.WriteLine("Flags:")
        Console.WriteLine("  -h" & vbTab & "--help" & vbTab & vbTab & "Show help menu")
        Console.WriteLine("  -v" & vbTab & "--version" & vbTab & "Show the current compiler version")
        Console.WriteLine("  -vb" & vbTab & "--verbose" & vbTab & "Add comments to the generated C file")
        Console.WriteLine("  -r" & vbTab & "--release" & vbTab & "Remove error handling runtime (like stacktrace)")
        Console.WriteLine("  -s" & vbTab & "--source" & vbTab & "Compile to C source file instead of an executable")
        Console.WriteLine("  -cc" & vbTab & "[path]" & vbTab & vbTab & "Specify the path to the C compiler (default: gcc)")

    End Sub

End Module
