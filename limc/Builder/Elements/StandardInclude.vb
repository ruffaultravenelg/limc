'
' Represents a header import into the C file (#include)
'
Imports System.IO

Public Class StandardInclude
    Inherits Include

    ' Library name
    Private LibraryName As String

    ' Constructor
    Public Sub New(LibraryName As String)
        Me.LibraryName = LibraryName
    End Sub

    ' Write the include
    Public Overrides Sub Write(Writer As StreamWriter)
        Writer.WriteLine("#include <" & LibraryName & ">")
    End Sub

End Class
