Public Class ClassConstructNode
    Inherits ConstructNode

    '===========================
    '======== PRIMITIVE ========
    '===========================
    Public ReadOnly Primitive As Boolean

    '============================
    '======== CLASS NAME ========
    '============================
    Public ReadOnly Name As String

    '===============================
    '======== GENERIC TYPES ========
    '===============================
    Private ReadOnly GenericTypes As List(Of GenericTypeNode)

    '=========================
    '======== CONTENT ========
    '=========================
    Private ReadOnly DeclareVariables As New List(Of DeclareVariableStatementNode)
    Private ReadOnly Getters As New List(Of GetterConstructNode)
    Private ReadOnly Setters As New List(Of SetterConstructNode)
    Private ReadOnly Methods As New List(Of MethodConstructNode)

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Private ReadOnly ReturnType As TypeNode

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, Primitive As Boolean, Name As String, GenericTypes As List(Of GenericTypeNode), Content As List(Of Node))
        MyBase.New(Location)

        'Set properties
        Me.Primitive = Primitive
        Me.Name = Name
        Me.GenericTypes = GenericTypes

        'Set content
        For Each Node As Node In Content

            If TypeOf Node Is DeclareVariableStatementNode Then
                DeclareVariables.Add(Node)
            ElseIf TypeOf node Is GetterConstructNode Then
                Getters.Add(Node)
            ElseIf TypeOf Node Is SetterConstructNode Then
                Setters.Add(Node)
            ElseIf TypeOf Node Is MethodConstructNode Then
                Methods.Add(Node)
            Else
                Throw New UnanticipatedException()
            End If

        Next

    End Sub

    '=========================
    '======== COMPILE ========
    '=========================
    Public Sub Compile()



    End Sub


End Class
