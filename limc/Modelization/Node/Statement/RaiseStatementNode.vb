Public Class RaiseStatementNode
    Inherits StatementNode

    '=======================
    '======== VALUE ========
    '=======================
    Private ReadOnly ExceptionName As String

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, ExceptionName As String)
        MyBase.New(Location)

        'Set value
        Me.ExceptionName = ExceptionName

    End Sub

    '=========================
    '======== COMPILE ========
    '=========================
    Public Overrides Sub Compile(Scope As Scope)
    End Sub

End Class
