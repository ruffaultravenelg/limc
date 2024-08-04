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

    '=========================
    '======== COMPILE ========
    '=========================
    Public Overrides Sub Compile(Scope As Scope)

        If VariableValue Is Nothing Then

            'let var:type
            Scope.CreateVariable(VariableName, VariableType.AssociatedType(Scope))

        ElseIf VariableType Is Nothing Then

            'let var = value
            Scope.CreateVariable(VariableName, VariableValue.ReturnType(Scope), VariableValue.Compile(Scope))

        Else

            'let var:type = value
            Dim VariableTrueType As Type = VariableType.AssociatedType(Scope)
            If Not VariableValue.CanReturn(VariableTrueType, Scope) Then
                Throw New TypeErrorException("The specified value cannot be cast to an """ & VariableTrueType.ToString() & """ type.", VariableValue.Location)
            End If
            Scope.CreateVariable(VariableName, VariableTrueType, VariableValue.Compile(Scope, VariableTrueType))

        End If

    End Sub

End Class
