Public Class SetterConstructNode
    Inherits LogicContainerConstruct

    '=============================
    '======== SETTER NAME ========
    '=============================
    Public ReadOnly Name As String

    '==================================
    '======== SETTER NEW VALUE ========
    '==================================
    Public ReadOnly NewValue As FunctionArgumentNode

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, Logic As List(Of StatementNode), Name As String, NewValue As FunctionArgumentNode)
        MyBase.New(Location, Logic)

        'Set properties
        Me.Name = Name
        Me.NewValue = NewValue

    End Sub

    '=========================
    '======== COMPILE ========
    '=========================
    Public Sub Compile()



    End Sub


End Class
