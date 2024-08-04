Public Class CallStatementNode
    Inherits StatementNode

    '============================
    '======== EXPRESSION ========
    '============================
    Private Expression As FunctionCallNode

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Expression As FunctionCallNode)
        MyBase.New(Expression.Location)

        'Set value
        Me.Expression = Expression

    End Sub

    '=========================
    '======== COMPILE ========
    '=========================
    Public Overrides Sub Compile(Scope As Scope)
        Scope.WriteLine(Expression.Compile(Scope) & ";")
    End Sub

End Class
