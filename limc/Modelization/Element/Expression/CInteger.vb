Public Class CInteger
    Inherits CExpression

    '========================
    '======== NUMBER ========
    '========================
    Private Value As Integer

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Value As Integer)
        MyBase.New()
        Me.Value = Value
    End Sub

    '==========================
    '======== ASSEMBLE ========
    '==========================
    Protected Overrides Function Compile() As String
        Return Value.ToString()
    End Function

End Class
