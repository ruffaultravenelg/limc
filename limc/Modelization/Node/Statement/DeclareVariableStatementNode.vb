Public Class DeclareVariableStatementNode
    Inherits StatementNode

    '===============================
    '======== VARIABLE NAME ========
    '===============================
    Public ReadOnly VariableName As String

    '===============================
    '======== VARIABLE TYPE ========
    '===============================
    Public ReadOnly VariableType As TypeNode

    '================================
    '======== VARIABLE VALUE ========
    '================================
    Public ReadOnly VariableValue As ExpressionNode

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, VariableName As String, VariableType As TypeNode, VariableValue As ExpressionNode)
        MyBase.New(Location)

        'Set values
        Me.VariableName = VariableName
        Me.VariableType = VariableType
        Me.VariableValue = VariableValue

    End Sub

End Class
