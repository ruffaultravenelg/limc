Imports System.IO

Namespace AST
    Public Class UseLibNode
        Inherits UseNode

        Public Overrides ReadOnly Property ModuleName As String

        Public Sub New(Location As Location, LibraryName As String)
            MyBase.New(Location)
            SetAbsoluteFilepath(Path.Combine(Compiler.COMPILER_DIRECTORY, "libs", LibraryName & ".lim"), LibraryName)
            ModuleName = LibraryName
        End Sub

    End Class
End Namespace