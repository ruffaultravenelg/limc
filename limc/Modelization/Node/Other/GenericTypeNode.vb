'
' Represent a genric type name
'    T
'    T:contract
'
Public Class GenericTypeNode
    Inherits Node

    '======================
    '======== NAME ========
    '======================
    Private ReadOnly Property Name As String

    '======================================
    '======== CONTRACT (optionnal) ========
    '======================================
    Private ReadOnly Property Contract As TypeNode

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, Name As String, Optional Contract As TypeNode = Nothing)
        MyBase.New(Location)

        'Set name
        Me.Name = Name

        'Set wanted contract
        Me.Contract = Contract

    End Sub

End Class
