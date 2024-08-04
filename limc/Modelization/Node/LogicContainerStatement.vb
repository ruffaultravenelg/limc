Public MustInherit Class LogicContainerStatement
    Inherits StatementNode

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location)
        MyBase.New(Location)
    End Sub

    '===============================
    '======== HAS STATEMENT ========
    '===============================
    Public MustOverride Function HasStatement(Of Statement As StatementNode)() As Boolean

End Class
