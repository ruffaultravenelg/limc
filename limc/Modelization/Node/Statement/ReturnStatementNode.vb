Public Class ReturnStatementNode
    Inherits StatementNode

    '=======================
    '======== VALUE ========
    '=======================
    Private ReadOnly Value As ExpressionNode

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, Value As ExpressionNode)
        MyBase.New(Location)

        'Set value
        Me.Value = Value

    End Sub

    '=========================
    '======== COMPILE ========
    '=========================
    Public Overrides Sub Compile(Scope As Scope)

        'Compile return
        Scope.WriteReturn(Value.Compile(Scope))

        'TODO: Convert newtypes with @converter

        'Set return value of upper function
        Scope.SetFunctionReturnType(Value.ReturnType(Scope), Value.Location)

    End Sub

End Class
