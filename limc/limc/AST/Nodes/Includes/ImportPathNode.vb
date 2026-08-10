Imports System.IO

Namespace AST
    Public Class ImportPathNode
        Inherits ImportNode

        Public Sub New(Location As Location, Filepath As String)
            MyBase.New(Location)
            SetAbsoluteFilepath(Path.GetFullPath(Filepath, Directory.GetParent(Location.File.Filepath).FullName), Filepath) 'Filepath is relative to current file
        End Sub

    End Class
End Namespace