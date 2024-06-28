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
        Me.Value = Integer.Parse(Value.Value)
    End Sub

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Protected Overrides Function CalculateReturnType(Context As Context) As Type
        Return Type.int
    End Function

    '==========================
    '======== ASSEMBLE ========
    '==========================
    Protected Overrides Function Assemble(Context As Context) As CExpression
        Return New CInteger(Value)
    End Function

End Class
