﻿Public Class ContinueStatementNode
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
        Scope.WriteLine("continue;")
    End Sub

End Class
