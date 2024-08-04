Imports System.IO

Public Module Compiler

    '===============================
    '======== COMPILE PATHS ========
    '===============================
    Private ReadOnly TempFolderPath As String = Path.Combine(Path.GetTempPath(), "limc")
    Private ReadOnly SourcePath As String = Path.Combine(TempFolderPath, "source.c")

    '===========================
    '======== MAIN FILE ========
    '===========================
    Private _MainFile As LimSource
    Public ReadOnly Property MainFile As LimSource
        Get
            Return _MainFile
        End Get
    End Property

    '====================================
    '======== COMPILE INPUT FILE ========
    '====================================
    Public Sub Compile()

        'Init main types
        Type.int = LimSource.STD.Type("int", {})

        'Parse main file
        _MainFile = New LimSource(Program.InputFile)

        'Start compiling from main()
        Dim MainFunction As CFunction = MainFile.Function("main", {}, {})

        'Create panic function
        CSourceFunction.GenerateSourceFunction(PanicFunctionPrototype, PanicFunctionContent)

        'Reset folder
        If Directory.Exists(TempFolderPath) Then
            Directory.Delete(TempFolderPath, True)
        End If
        Directory.CreateDirectory(TempFolderPath)

        'Create result file
        Dim FileWriter As New StreamWriter(SourcePath)

        'Build file
        FileBuilder.BuildFile(FileWriter)

        'Close writer
        FileWriter.Close()

    End Sub

    '================================
    '======== PANIC FUNCTION ========
    '================================
    Private Const PanicFunctionPrototype As String = "void lim_panic(const char *format, ...)"
    Private ReadOnly PanicFunctionContent As IEnumerable(Of String) = {
        "va_list args;",
        "fprintf(stderr, ""\x1b[31mLIM RUNTIME ERROR: \x1b[0m"");",
        "va_start(args, format);",
        "vfprintf(stderr, format, args);",
        "va_end(args);",
        "fprintf(stderr, ""\n"");",
        "exit(-1);"
    }

    '==============================================
    '======== EXCEPTION DURING COMPILATION ========
    '==============================================
    Public Class CompilerException
        Inherits Exception

        Private Name As String

        Public Sub New(Name As String, Message As String)
            MyBase.New(Message)
            Me.Name = Name
        End Sub

        Public Sub Print()

            'Print content
            PrintContent()

            'Reset console color
            Console.ResetColor()

        End Sub
        Protected Overridable Sub PrintContent()

            'Print title
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("ERROR: " & Name)

            'Print message
            Console.ResetColor()
            Console.WriteLine(Message)

        End Sub


    End Class

End Module
