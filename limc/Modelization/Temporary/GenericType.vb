'
' Represent a defined generic type
' Example: T = list<int>
'
Public Class GenericType

    '======================
    '======== NAME ========
    '======================
    Public ReadOnly Property Name As String

    '======================
    '======== TYPE ========
    '======================
    Public ReadOnly Property Type As Type

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Name As String, Type As Type)
        Me.Name = Name
        Me.Type = Type
    End Sub

End Class
