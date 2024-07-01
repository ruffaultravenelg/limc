Public Class MethodConstructNode
    Inherits LogicContainerConstruct

    '============================
    '======== CLASS NAME ========
    '============================
    Public ReadOnly Name As String

    '===========================
    '======== ARGUMENTS ========
    '===========================
    Private ReadOnly Arguments As List(Of FunctionArgumentNode)

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Private ReadOnly ReturnType As TypeNode

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, Logic As List(Of StatementNode), Name As String, Arguments As List(Of FunctionArgumentNode), ReturnType As TypeNode)
        MyBase.New(Location, Logic)

        'Set properties
        Me.Name = Name
        Me.Arguments = Arguments
        Me.ReturnType = ReturnType

    End Sub

    '=========================
    '======== COMPILE ========
    '=========================
    Public Sub Compile()



    End Sub


End Class
