Public Class GetterConstructNode
    Inherits LogicContainerConstruct

    '=============================
    '======== GETTER NAME ========
    '=============================
    Public ReadOnly Name As String

    '=============================
    '======== GETTER TYPE ========
    '=============================
    Private ReadOnly ReturnType As TypeNode

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, Logic As List(Of StatementNode), Name As String, ReturnType As TypeNode)
        MyBase.New(Location, Logic)

        'Set properties
        Me.Name = Name
        Me.ReturnType = ReturnType

    End Sub

    '=========================
    '======== COMPILE ========
    '=========================
    Public Sub Compile()



    End Sub


End Class
