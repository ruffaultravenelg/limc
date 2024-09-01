'
' Represent a class's variable
' Can only be modified in a class's method
'
Public Class Propertie

    '===============================
    '======== COMPILED NAME ========
    '===============================
    Public ReadOnly Property CompiledName As String

    '=============================
    '======== ACCESS NAME ========
    '=============================
    Public ReadOnly Property AcessName As String

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
    Public Sub New(ParentType As ClassType, Name As String, Type As Type, CompiledName As String)

        'Set compiled name
        Me.CompiledName = CompiledName

        'Set acess name
        Me.AcessName = "((" & ParentType.Name & "*)self)->" & CompiledName

        'Set name
        Me.Name = Name

        'Set type
        Me.Type = Type

    End Sub

End Class
