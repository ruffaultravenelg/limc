Imports System.IO

Namespace AST
    Public Class UsePathNode
        Inherits UseNode

        Public Overrides ReadOnly Property ModuleName As String

        Public Sub New(Location As Location, Filepath As String, ModuleName As String)
            MyBase.New(Location)
            SetAbsoluteFilepath(Path.GetFullPath(Filepath, Directory.GetParent(Location.File.Filepath).FullName), Filepath) 'Filepath is relative to current file
            Me.ModuleName = ModuleName
        End Sub

    End Class
End Namespace