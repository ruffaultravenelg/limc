Imports System.IO
Imports limc.AST

Public Class SourceFile

    '======================================
    '===== ALL REGISTERED SOURCE FILE =====
    '======================================
    Private Shared SolutionFiles As New List(Of SourceFile)

    '======================
    '===== PROPERTIES =====
    '======================
    Public ReadOnly Property Filepath As String
    Public Property AST As AST.AbstractSyntaxTree

    Public ReadOnly Property Filename As String
        Get
            Return Path.GetFileName(Filepath)
        End Get
    End Property

    '=======================
    '===== CONSTRUCTOR =====
    '=======================
    Private Sub New(Filepath As String)
        Me.Filepath = Filepath
    End Sub

    Public Shared Function FromFile(Filepath As String) As SourceFile

        'Sanatize path
        Filepath = Path.GetFullPath(Filepath)

        'Search if file is already registered, return it
        For Each SolutionFile As SourceFile In SourceFile.SolutionFiles
            If SolutionFile.Filepath = Filepath Then
                Return SolutionFile
            End If
        Next

        'Create and register instance
        Dim SourceInstance As New SourceFile(Filepath)
        SolutionFiles.Add(SourceInstance)

        'Read file
        Dim Tokens As IEnumerable(Of Token) = Tokenizer.TokenizeFile(SourceInstance)

        'For Each Tok As Token In Tokens
        '   If Tok.Type = TokenType.LINESTART Then
        '       Console.WriteLine()
        '   End If
        '   Console.Write(Tok.ToString())
        'Next

        'Generate AST
        SourceInstance.AST = New AbstractSyntaxTree(Tokens)

        'Return object instance
        Return SourceInstance

    End Function

End Class
