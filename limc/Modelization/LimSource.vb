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
    Public ReadOnly Exceptions As New List(Of Node)

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

        'File format
        Select Case Path.GetExtension(Filepath)
            Case ".lim"
                ParseFromText()
            Case ".limlib"
                ParseFromNode()
            Case ".json"
                ParseFromJson()
        End Select

        'Import std.lim if the current file is not std.lim
        If Not (HasTheSamePathAs(Path.Combine(Platform.LibDirectory, "std.lim"))) Then
            ImportedFiles.Add(LimSource.STD)
        End If

        'Compilation target
        If Program.CompilationTarget = CompilationType.Libs Then
            NodeSerializer.SaveSourceToFile(Me, Program.OutputFile & ".limlib")
            End
        ElseIf Program.CompilationTarget = CompilationType.Json Then
            NodeSerializer.SaveSourceToJsonFile(Me, Program.OutputFile & ".json")
            End
        End If

    End Sub

    '=================================
    '======== PARSE FROM TEXT ========
    '=================================
    Private Sub ParseFromText()

        'Get lines
        Dim Lines As List(Of LimSourceLine) = LineParser.Parse(Filepath)

        'Convert to tokens
        Dim Tokens As List(Of Token) = TokenParser.Parse(Me, Lines)

        'Parse nodes
        NodeParser.Parse(Me, Tokens)

    End Sub

    '=================================
    '======== PARSE FROM NODE ========
    '=================================
    Private Sub ParseFromNode()
        NodeSerializer.LoadSourceFile(Me, Filepath)
    End Sub

    '=================================
    '======== PARSE FROM JSON ========
    '=================================
    Private Sub ParseFromJson()
        NodeSerializer.LoadSourceJsonFile(Me, Filepath)
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


End Class
