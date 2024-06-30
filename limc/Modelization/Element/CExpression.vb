Public MustInherit Class CExpression

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New()

    End Sub

    '=========================
    '======== COMPILE ========
    '=========================
    Protected MustOverride Function Compile() As String

End Class
