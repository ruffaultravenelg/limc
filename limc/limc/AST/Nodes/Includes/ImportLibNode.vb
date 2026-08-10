Imports System.IO

Namespace AST
    Public Class ImportLibNode
        Inherits ImportNode

        Public Sub New(Location As Location, LibraryName As String)
            MyBase.New(Location)
            SetAbsoluteFilepath(Path.Combine(Compiler.COMPILER_DIRECTORY, "libs", LibraryName & ".lim"), LibraryName)
        End Sub

    End Class
End Namespace