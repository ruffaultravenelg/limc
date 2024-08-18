'
' Represents a header import into the C file (#include)
'
Imports System.IO

Public MustInherit Class Include

    ' List all includes
    Protected Shared Includes As New List(Of Include)

    ' Add a new includes
    Public Shared Sub Add(Include As Include)
        Includes.Add(Include)
    End Sub

    ' Write all includes
    Public Shared Sub WriteAllIncludes(Writer As StreamWriter)
        For Each Include As Include In Includes
            Include.Write(Writer)
        Next
    End Sub

    ' Write the include
    Public MustOverride Sub Write(Writer As StreamWriter)

End Class
