Public MustInherit Class LogicContainerConstruct
    Inherits ConstructNode

    '============================
    '======== HAS RETURN ========
    '============================
    Public ReadOnly Property HasReturn As Boolean

    '===========================
    '======== HAS RAISE ========
    '===========================
    Public ReadOnly Property HasRaise As Boolean

    '=======================
    '======== LOGIC ========
    '=======================
    Public ReadOnly Logic As List(Of StatementNode)

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Protected Sub New(Location As Location, Logic As List(Of StatementNode))
        MyBase.New(Location)

        'Set logic
        Me.Logic = Logic

        'Check return
        HasReturn = StatementNode.ListContains(Of ReturnStatementNode)(Logic)

        'Check raise
        HasReturn = StatementNode.ListContains(Of RaiseStatementNode)(Logic)

    End Sub

End Class
