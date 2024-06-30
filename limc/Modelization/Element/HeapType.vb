Public Class HeapType
    Inherits Type

    '==================================
    '======== REMOVE REFERENCE ========
    '==================================
    Public Sub RemoveReference(Result As List(Of String), Target As String)
        Result.Add($"{Name}_removeReference({Target});")
    End Sub

    '===============================
    '======== ADD REFERENCE ========
    '===============================
    Public Sub AddsReference(Result As List(Of String), Target As String)
        Result.Add($"{Name}_addReference({Target});")
    End Sub

    '===============================
    '======== COMPILED NAME ========
    '===============================
    Public Overrides ReadOnly Property CompiledName As String
        Get
            Return MyBase.CompiledName & "*"
        End Get
    End Property

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New()
        MyBase.New()

    End Sub

End Class
