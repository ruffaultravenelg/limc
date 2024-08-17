Public Class BooleanNode
    Inherits ExpressionNode

    '=======================
    '======== VALUE ========
    '=======================
    Private Value As Boolean

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Value As Token)
        MyBase.New(Value.Location)

        'Set values
        Me.Value = If(Value.Value = "true", True, False)

    End Sub

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Protected Overrides Function CalculateReturnType(Scope As Scope) As Type
        Return Type.bool
    End Function

    '==========================
    '======== ASSEMBLE ========
    '==========================
    Protected Overrides Function Assemble(Scope As Scope) As String
        Return If(Value, "true", "false")
    End Function

End Class
