Public MustInherit Class LogicContainerConstruct
    Inherits ConstructNode

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

    End Sub

End Class
