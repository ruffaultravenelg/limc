Public Class CGetter
    Inherits Context

    '======================
    '======== NAME ========
    '======================
    Private Name As String

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Name As String)
        MyBase.New()
        Me.Name = Name
    End Sub

    '==========================
    '======== ASSEMBLE ========
    '==========================
    Protected Overrides Sub Assemble(Result As List(Of String))

    End Sub

    '============================
    '======== CORRESPOND ========
    '============================
    Public Function Correspond(Name As String) As Boolean
        Return Me.Name = Name
    End Function

End Class
