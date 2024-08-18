'
' Represents a header import into the C file (#include)
'
Imports System.IO

Public Class CustomInclude
    Inherits Include

    ' Library name
    Private CustomName As String

    ' Constructor
    Public Sub New(CustomName As String)
        Me.CustomName = Path.GetFullPath(CustomName)
    End Sub

    ' All
    Public Shared ReadOnly Iterator Property All As IEnumerable(Of String)
        Get
            For Each Include As Include In Include.Includes
                If TypeOf Include Is CustomInclude Then
                    Yield DirectCast(Include, CustomInclude).CustomName
                End If
            Next
        End Get
    End Property

    ' Write the include
    Public Overrides Sub Write(Writer As StreamWriter)
        Writer.WriteLine("#include """ & CustomName & """")
    End Sub

End Class