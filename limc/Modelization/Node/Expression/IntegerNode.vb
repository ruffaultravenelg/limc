Public Class IntegerNode
    Inherits ExpressionNode

    '=======================
    '======== VALUE ========
    '=======================
    Private Value As Integer

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Value As Token)
        MyBase.New(Value.Location)

        'Set values
        Me.Value = Integer.Parse(Value.Value)

    End Sub

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Protected Overrides Function CalculateReturnType(Scope As Scope) As Type
        Return Type.int
    End Function

    '==========================
    '======== ASSEMBLE ========
    '==========================
    Protected Overrides Function Assemble(Scope As Scope) As String
        Return Value.ToString()
    End Function

End Class
