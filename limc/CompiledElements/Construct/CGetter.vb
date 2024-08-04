Public Class CGetter

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
    Private Function Build() As List(Of String)

        'Create result
        Dim Result As New List(Of String)

        'Return result
        Return Result

    End Function

    '============================
    '======== CORRESPOND ========
    '============================
    Public Function Correspond(Name As String) As Boolean
        Return Me.Name = Name
    End Function

End Class
