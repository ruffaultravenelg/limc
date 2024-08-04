Public Class OperationNode
    Inherits ExpressionNode

    '============================
    '======== LEFT VALUE ========
    '============================
    Private LeftValue As ExpressionNode

    '==========================
    '======== OPERATOR ========
    '==========================
    Private Op As TokenType

    '=============================
    '======== RIGHT VALUE ========
    '=============================
    Private RightValue As ExpressionNode

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(LeftValue As ExpressionNode, Op As TokenType, RightValue As ExpressionNode)
        MyBase.New(LeftValue.Location & RightValue.Location)

        'Set values
        Me.LeftValue = LeftValue
        Me.Op = Op
        Me.RightValue = RightValue

    End Sub

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Protected Overrides Function CalculateReturnType(Scope As Scope) As Type

        Return Nothing

    End Function

    '==========================
    '======== ASSEMBLE ========
    '==========================
    Protected Overrides Function Assemble(Scope As Scope) As String

        Return Nothing

    End Function

End Class
