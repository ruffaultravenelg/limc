Imports System.IO

Public Class LimSource

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(filepath As String)

        'Get full path
        filepath = Path.GetFullPath(filepath)

        'File doesn't exist
        If Not File.Exists(filepath) Then
            Throw New FileDoNotExistException("File """ & Path.GetRelativePath(".", filepath) & """ doesn't exist.")
        End If

        'Get lines
        Dim Lines As List(Of LimSourceLine) = LineParser.Parse(filepath)

        For Each line As LimSourceLine In Lines
            Console.WriteLine($"{line.LinePosition} | {line.Tab}> {line.Content}")
        Next

    End Sub

End Class
