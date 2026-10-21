Friend Class ProgramDirective

    ' Shared constructor (more readable)
    Public Shared Function FromArgs(Args As String()) As ProgramDirective
        Return New ProgramDirective(Args)
    End Function

    ' I/O
    Public ReadOnly Property InputFilename As String = String.Empty
    Public ReadOnly Property OutputFilename As String = String.Empty

    ' Options
    Public ReadOnly Property Target As CompilationTarget = CompilationTarget.Executable

    ' Flags
    Public ReadOnly Property HelpFlag As Boolean = False
    Public ReadOnly Property VersionFlag As Boolean = False

    ' Constructor / Argument parser
    Private Sub New(Args As String())

        ' Parse args
        For Each arg In Args

            If arg.StartsWith("-") Then

                'flag
                Select Case arg.ToLower()
                    Case "-h", "--help"
                        _HelpFlag = True
                    Case "-v", "--version"
                        _VersionFlag = True

                    Case "-e", "--executable"
                        _Target = CompilationTarget.Executable
                    Case "-c", "--c"
                        _Target = CompilationTarget.C
                    Case "-t", "--token"
                        _Target = CompilationTarget.Token
                    Case "-a", "--ast"
                        _Target = CompilationTarget.AST
                    Case "-i", "--hir"
                        _Target = CompilationTarget.HIR

                    Case Else
                        Throw New ArgumentParsingException("AP02", "Unknown option/flag")
                End Select

            Else

                ' word
                If String.IsNullOrEmpty(InputFilename) Then
                    InputFilename = arg
                ElseIf String.IsNullOrEmpty(OutputFilename) Then
                    OutputFilename = arg
                Else
                    Throw New ArgumentParsingException("AP01", "Input and Output are already specified, nothing else is needed.")
                End If

            End If

        Next

        ' Generate I/O if needed
        If NeedsInputOutput() Then

            If String.IsNullOrEmpty(InputFilename) Then
                Throw New ArgumentParsingException("AP03", "No input file specified. Use -h to learn more.")
            ElseIf Not IO.File.Exists(InputFilename) Then
                Throw New ArgumentParsingException("AP04", $"Input file doesn't exist : ""{InputFilename}"".")
            End If

            If String.IsNullOrEmpty(OutputFilename) Then
                OutputFilename = GenerateOutputFilename()
            End If

        End If

    End Sub

    Private Function NeedsInputOutput() As Boolean
        Return Not (HelpFlag OrElse VersionFlag)
    End Function

    Private Function GenerateOutputFilename() As String
        Dim InputName As String = IO.Path.GetFileNameWithoutExtension(InputFilename)
        Dim OutputExtension As String = GetOutputFileExtension()

        Dim Filename As String = InputName & OutputExtension

        ' Create a filename that is not used
        ' Dim counter As Integer = 0
        ' While IO.File.Exists(Filename)
        '     Filename = InputName & counter.ToString() & OutputExtension
        '     counter += 1
        ' End While

        Return Filename
    End Function

    Private Function GetOutputFileExtension() As String
        Select Case Target
            Case CompilationTarget.C
                Return ".c"
            Case CompilationTarget.Executable
                Return ""
            Case CompilationTarget.Token
                Return ".tokens.txt"
            Case CompilationTarget.AST
                Return ".ast.txt"
            Case CompilationTarget.HIR
                Return ".hir.txt"
            Case Else
                Return ".txt"
        End Select
    End Function


    Public Enum CompilationTarget
        Executable
        C
        Token
        AST
        HIR
    End Enum

End Class
