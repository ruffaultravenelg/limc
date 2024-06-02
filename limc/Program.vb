Imports System.IO

Module Program

    '==================================
    '======== COMPILATION TYPE ========
    '==================================
    Public Enum CompilationType
        Binary
        C
        Tokens
        Libs
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
    '======== CUSTOM TAGS ========
    '=============================
    Private _DefinedFlag As New List(Of String)
    Public ReadOnly Property DefinedFlag As List(Of String)
        Get
            Return _DefinedFlag
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
#If DEBUG = False Then
        Catch ex As Exception
            HandleSelfError()
#End If
        End Try
    End Sub
    Sub SubMain(args As String())

        'Single action
        If args.Count = 1 Then
            If args(0) = "-v" OrElse args(0) = "--version" Then
                ShowVersion()
                Return
            ElseIf args(0) = "-h" OrElse args(0) = "--help" Then
                ShowHelp()
                Return
            End If
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

        'Get full path of final file
        _OutputFile = Path.GetFullPath(_OutputFile)

        'Flags but compiles a lib
        If DefinedFlag.Count > 0 AndAlso (CompilationTarget = CompilationType.Libs OrElse CompilationTarget = CompilationType.Tokens) Then
            Throw New InvalidCompileTargetException("You are compiling to a library file while limiting the contents of the file with flags. When you want to compile to this kind of file, no compilation flag can be set.")
        End If

        'Add current platform flag
        _DefinedFlag.Add(Platform.Current.ToString())

        'Compile to .limtok
        If CompilationTarget = CompilationType.Tokens Then
            TokenSerializer.ParseFileAndSaveTokens(InputFile, OutputFile)
        End If

        'Start to compile
        Compiler.Compile()

    End Sub

    '===================================
    '======== HANDLE SELF ERROR ========
    '===================================
    Private Sub HandleSelfError()

        'Write error titl
        Console.ForegroundColor = ConsoleColor.DarkRed
        Console.WriteLine("INTERNAL ERROR: It seems that an unexpected error has occurred")

        'Write message
        Console.ResetColor()
        Console.WriteLine("It's likely that this is due to your configuration. In any case, it's very likely that it's a compiler bug and not an error on your behalf.")

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
                    Case "tok"
                        _CompilationTarget = CompilationType.Tokens
                    Case "lib"
                        _CompilationTarget = CompilationType.Libs
                    Case Else
                        Throw New ArgumentException("Unknown compilation target """ & args(i) & """")
                End Select
                Continue While
            End If

            'Custom flag
            If arg = "-f" OrElse arg = "--add-flag" Then
                i += 1
                _DefinedFlag.Add(args(i))
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
        Console.WriteLine(vbTab & "--target [<com_type>]" & vbTab & "-t [<com_type>]" & vbTab & vbTab & ": Set file type to be compiled.")
        Console.WriteLine(vbTab & "--add-flag [flag]" & vbTab & "-f [flag]" & vbTab & vbTab & ": Add a custom flag.")
        Console.WriteLine("")
        Console.WriteLine("<com_type>:")
        Console.WriteLine(vbTab & "bin" & vbTab & ": Compiles the project to an executable")
        Console.WriteLine(vbTab & "c" & vbTab & ": Compiles the project to a single .c source file")
        Console.WriteLine(vbTab & "tok" & vbTab & ": Compiles the file to a .limtok file, containing all tokens")
        Console.WriteLine(vbTab & "lib" & vbTab & ": Compiles the file to a .limlib file, containing all the code")

    End Sub

    '==============================
    '======== SHOW VERSION ========
    '==============================
    Private Sub ShowVersion()

        'Version
        Console.WriteLine("Lim compiler version " & Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString())

    End Sub

End Module
