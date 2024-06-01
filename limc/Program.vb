Imports System.IO

Module Program

    '==================================
    '======== COMPILATION TYPE ========
    '==================================
    Public Enum CompilationType
        Binary
        C
    End Enum
    Private _CompilationTarget As CompilationType = CompilationType.Binary
    Public ReadOnly Property CompilationTarget As CompilationType
        Get
            Return _CompilationTarget
        End Get
    End Property

    '============================
    '======== INPUT FILE ========
    '============================
    Private _InputFile As String = ""
    Public ReadOnly Property InputFile As String
        Get
            Return _InputFile
        End Get
    End Property

    '=============================
    '======== OUTPUT FILE ========
    '=============================
    Private _OutputFile As String = ""
    Public ReadOnly Property OutputFile As String
        Get
            Return _OutputFile
        End Get
    End Property


    '=============================
    '======== ENTRY POINT ========
    '=============================
    Sub Main(args As String())
        Try
            SubMain(args)
        Catch ex As CompilerException
            ex.Print()
        End Try
    End Sub
    Sub SubMain(args As String())

        'Single action
        If args.Count = 1 Then
            If args(0) = "-v" OrElse args(0) = "--version" Then
                ShowVersion()
            ElseIf args(0) = "-h" OrElse args(0) = "--help" Then
                ShowHelp()
            End If
            Return
        End If

        'Get arguments
        Try
            HandleArgument(args)
        Catch ex As IndexOutOfRangeException
            Throw New ArgumentException("A value was excpected after """ & args(args.Count - 1) & """")
        End Try

        'No input file
        If _InputFile = "" Then
            Throw New ArgumentException("No file to compile has been supplied.")
        End If

        'No output file
        If _OutputFile = "" Then
            _OutputFile = Path.GetFileNameWithoutExtension(_InputFile)
        End If

    End Sub

    '==================================
    '======== HANDLE ARGUMENTS ========
    '==================================
    Private Sub HandleArgument(args As String())

        Dim i As Integer = -1
        While i < args.Count - 1
            i += 1
            Dim arg As String = args(i)

            'File name
            If Not arg.StartsWith("-") Then

                'Input file
                If _InputFile = "" Then
                    _InputFile = arg
                    Continue While
                End If

                'Output file
                If _OutputFile = "" Then
                    _OutputFile = arg
                    Continue While
                End If

                'Not intended
                Throw New ArgumentException("The """ & arg & """ argument was not expected.")

            End If

            'Target
            If arg = "-t" OrElse arg = "--target" Then
                i += 1
                Select Case args(i)
                    Case "bin"
                        _CompilationTarget = CompilationType.Binary
                    Case "c"
                        _CompilationTarget = CompilationType.C
                    Case Else
                        Throw New ArgumentException("Unknown compilation target """ & args(i) & """")
                End Select
                Continue While
            End If

            'Not found
            Throw New ArgumentException("The """ & arg & """ flag is unknown or unexpected at this location.")

        End While

    End Sub

    '===========================
    '======== SHOW HELP ========
    '===========================
    Private Sub ShowHelp()

        'Usage
        Console.WriteLine("Usage: limc <input> [output] [flags...]")
        Console.WriteLine(vbTab & "<input>" & vbTab & vbTab & ": The Lim source file")
        Console.WriteLine(vbTab & "[output]" & vbTab & ": The name of the output executable")
        Console.WriteLine(vbTab & "[flags...]" & vbTab & ": A list of different flags")

        'Flags
        Console.WriteLine("")
        Console.WriteLine("Flags:")
        Console.WriteLine(vbTab & "--help" & vbTab & vbTab & vbTab & "-h" & vbTab & vbTab & vbTab & ": Show help menu")
        Console.WriteLine(vbTab & "--version" & vbTab & vbTab & "-v" & vbTab & vbTab & vbTab & ": Show current software version")
        Console.WriteLine("")
        Console.WriteLine(vbTab & "--icon [icon_path]" & vbTab & "-i [icon_path]" & vbTab & vbTab & ": Set the executable icon")
        Console.WriteLine(vbTab & "--gcc-bin [gcc_bin_dir]" & vbTab & "-gcc [gcc_bin_dir]" & vbTab & ": Set a custom gcc bin directory. Just the parent folder.")
        Console.WriteLine(vbTab & "--target [bin/c]" & vbTab & "-t [bin/c]" & vbTab & vbTab & ": Set file type to be compiled.")

    End Sub

    '==============================
    '======== SHOW VERSION ========
    '==============================
    Private Sub ShowVersion()

        'Version
        Console.WriteLine("Lim compiler version " & Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString())

    End Sub

End Module
