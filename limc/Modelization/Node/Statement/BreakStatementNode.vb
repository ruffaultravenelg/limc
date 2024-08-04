Public Class BreakStatementNode
    Inherits StatementNode

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location)
        MyBase.New(Location)
    End Sub

    '=========================
    '======== COMPILE ========
    '=========================
    Public Overrides Sub Compile(Scope As Scope)
        Scope.WriteLine("break;")
    End Sub

End Class
