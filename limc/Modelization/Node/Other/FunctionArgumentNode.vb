'
' Represent a argument
'    name:type
'
Public Class FunctionArgumentNode
    Inherits Node

    '======================
    '======== NAME ========
    '======================
    Public ReadOnly Property Name As String

    '======================
    '======== TYPE ========
    '======================
    Public ReadOnly Property Type As TypeNode

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, Name As String, Type As TypeNode)
        MyBase.New(Location)

        'Set name
        Me.Name = Name

        'Set type
        Me.Type = Type

    End Sub

End Class
