Imports System.IO

Public Class LimSource

    '===========================
    '======== FILE PATH ========
    '===========================
    Public ReadOnly Filepath As String

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(filepath As String)

        'Get full path
        filepath = Path.GetFullPath(filepath)
        Me.Filepath = filepath

        'File doesn't exist
        If Not File.Exists(filepath) Then
            Throw New FileDoNotExistException("File """ & Path.GetRelativePath(".", filepath) & """ doesn't exist.")
        End If

        'Get lines
        Dim Lines As List(Of LimSourceLine) = LineParser.Parse(filepath)

    End Sub

End Class
