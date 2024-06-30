Imports System.IO

Public Class LimSource

    '====================================
    '======== EVERY PARSED FILES ========
    '====================================
    Private Shared ReadOnly EveryParsedFiles As New List(Of LimSource) From {}

    '============================
    '======== MAIN FILES ========
    '============================
    Public Shared ReadOnly STD As LimSource = New LimSource(Path.Combine(Platform.LibDirectory, "std.lim"))

    '===========================
    '======== FILE PATH ========
    '===========================
    Public ReadOnly Filepath As String

    '============================
    '======== EXCEPTIONS ========
    '============================
    Public ReadOnly Exceptions As New List(Of ExceptionConstructNode)

    '===========================
    '======== FUNCTIONS ========
    '===========================
    Public ReadOnly Functions As New List(Of FunctionConstructNode)

    '=========================
    '======== IMPORTS ========
    '=========================
    Public ReadOnly ImportedFiles As New List(Of LimSource)

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Filepath As String)

        'Add itself to the list of compiled file
        EveryParsedFiles.Add(Me)

        'Get full path
        Filepath = Path.GetFullPath(Filepath)
        Me.Filepath = Filepath

        'File doesn't exist
        If Not File.Exists(Filepath) Then
            Throw New FileDoNotExistException("File """ & Path.GetRelativePath(".", Filepath) & """ doesn't exist.")
        End If

        'Get lines
        Dim Lines As List(Of LimSourceLine) = LineParser.Parse(Filepath)

        'Convert to tokens
        Dim Tokens As List(Of Token) = TokenParser.Parse(Me, Lines)

        'Parse nodes
        NodeParser.Parse(Me, Tokens)

        'Export all constructs if no one is exported
        Dim ExportedConstructFound As Boolean = False
        For Each elm As ConstructNode In Constructs
            If elm.Exported Then
                ExportedConstructFound = True
                Exit For
            End If
        Next
        If Not ExportedConstructFound Then
            For Each elm As ConstructNode In Constructs
                elm.Exported = True
            Next
        End If

        'Import std.lim if the current file is not std.lim
        If Not (HasTheSamePathAs(Path.Combine(Platform.LibDirectory, "std.lim"))) Then
            ImportedFiles.Add(LimSource.STD)
        End If

        'Compilation target
        If Program.CompilationTarget = CompilationType.Libs Then
            Throw New NotImplementedException
            End
        ElseIf Program.CompilationTarget = CompilationType.Json Then
            Throw New NotImplementedException
            End
        End If

        ' Tests
        For Each fun As FunctionConstructNode In Functions
            Console.WriteLine(fun.Name & " : " & fun.Exported.ToString)
        Next
        For Each exception As ExceptionConstructNode In Exceptions
            Console.WriteLine(exception.Name & " : " & exception.Exported.ToString)
        Next


    End Sub

    '===============================
    '======== IMPORT A FILE ========
    '===============================
    Private Sub Import(Filename As String)

        'Get full path
        Filename = Path.GetFullPath(Filename)

        'Search if file is already imported in imported files
        For Each File As LimSource In ImportedFiles
            If File.HasTheSamePathAs(Filename) Then
                Return
            End If
        Next

        'Search if the file is already compiled
        For Each File As LimSource In LimSource.EveryParsedFiles
            If File.HasTheSamePathAs(Filename) Then
                ImportedFiles.Add(File)
                Return
            End If
        Next

        'Compile the file
        ImportedFiles.Add(New LimSource(Filename))

    End Sub

    '=============================
    '======== IS THE SAME ========
    '=============================
    Private Function HasTheSamePathAs(Other As String)
        Return Filepath = Other
    End Function

    '===================================
    '======== FILE DO NOT EXIST ========
    '===================================
    Public Class FileDoNotExistException
        Inherits CompilerException

        Public Sub New(Message As String)
            MyBase.New("File doesn't exist", Message)
        End Sub

    End Class

    '====================================
    '======== CONSTRUCT ITERATOR ========
    '====================================
    Private ReadOnly Iterator Property Constructs As IEnumerable(Of ConstructNode)
        Get
            For Each elm In Exceptions
                Yield elm
            Next
            For Each elm In Functions
                Yield elm
            Next
        End Get
    End Property

End Class
