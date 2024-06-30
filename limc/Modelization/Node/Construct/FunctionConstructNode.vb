Public Class FunctionConstructNode
    Inherits LogicContainerConstruct

    '===============================
    '======== FUNCTION NAME ========
    '===============================
    Public ReadOnly Name As String

    '===============================
    '======== GENERIC TYPES ========
    '===============================
    Private ReadOnly GenericTypes As List(Of GenericTypeNode)

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
    Public Sub New(Location As Location, Logic As List(Of StatementNode), Name As String, GenericTypes As List(Of GenericTypeNode), Arguments As List(Of FunctionArgumentNode), Optional ReturnType As TypeNode = Nothing)
        MyBase.New(Location, Logic)

        'Set properties
        Me.Name = Name
        Me.GenericTypes = GenericTypes
        Me.Arguments = Arguments
        Me.ReturnType = ReturnType

    End Sub

    '=========================
    '======== COMPILE ========
    '=========================
    Public Sub Compile()



    End Sub


End Class
