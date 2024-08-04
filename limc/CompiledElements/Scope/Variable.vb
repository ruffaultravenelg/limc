'
' Represent a variable of a context
'
Public Class Variable

    '===============================
    '======== COMPILED NAME ========
    '===============================
    Private Shared VariableCount As Integer = 0
    Public ReadOnly Property CompiledName As String

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

        'Set compiled name
        VariableCount += 1
        CompiledName = "var" & VariableCount

        'Set name
        Me.Name = Name

        'Set type
        Me.Type = Type

    End Sub
    Public Sub New(Name As String, Type As Type, CompiledName As String)

        'Set compiled name
        Me.CompiledName = CompiledName

        'Set name
        Me.Name = Name

        'Set type
        Me.Type = Type

    End Sub

End Class
