'
' Represent a type
'    list<str>
'
Public Class TypeNode
    Inherits Node

    '======================
    '======== NAME ========
    '======================
    Private ReadOnly Property Name As String

    '======================================
    '======== PASSED GENERIC TYPES ========
    '======================================
    Private ReadOnly Property PassedGenericTypes As List(Of TypeNode)

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, Name As String, PassedGenericTypes As List(Of TypeNode))
        MyBase.New(Location)

        'Set name
        Me.Name = Name

        'Set passed generic types
        Me.PassedGenericTypes = PassedGenericTypes

    End Sub

End Class
