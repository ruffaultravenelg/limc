Imports System.IO

Module Program

    'Program version
    Private Const VERSION As Double = 0.1

    'Main entry point
    Sub Main(args As String())
        Try

            'Load arguments
            ArgumentHandler.LoadArgs(args)

            'Display help
            If ArgumentHandler.Help Then
                DisplayHelp()
                Exit Sub
            End If

            'Display version
            If ArgumentHandler.Version Then
                DisplayVersion()
                Exit Sub
            End If

            'Check input
            If ArgumentHandler.Input = Nothing Then
                Throw New SimpleException("No input", "No input file specified.")
            End If

            'Compile
            Compile()

        Catch ex As DisplayableException

            'Display exception and quit
            ex.Display()

        End Try
    End Sub

    'Display help
    Private Sub DisplayHelp()
        Console.WriteLine("Usage: limc <input> [output] [flags...] ")
        Console.WriteLine("Flags:")
        Console.WriteLine("  -h, --help     Display this help message")
        Console.WriteLine("  -v, --version  Display version")
        Console.WriteLine("  -l, --libs     Change libs path to specified directory")
        Console.WriteLine("  -g, --gcc      Set gcc executable path")
    End Sub

    'Display version
    Private Sub DisplayVersion()
        Console.WriteLine("Lim Compiler (limc) - Version " & VERSION)
    End Sub

    'Compile
    Private Sub Compile()

        ' Get temp path
        Dim TempPath As String = Path.Combine(IO.Path.GetTempPath(), "limc")

        ' Create directory
        If Not IO.Directory.Exists(TempPath) Then
            IO.Directory.CreateDirectory(TempPath)
        End If

        ' Create source.c
        Dim Source As String = Path.Combine(TempPath, "source.c")

        'Remove file if exists
        If IO.File.Exists(Source) Then
            IO.File.Delete(Source)
        End If

        ' Create compiler
        Compiler.Compile(ArgumentHandler.Input, Source)

    End Sub

End Module
