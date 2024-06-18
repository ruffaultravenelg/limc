Public MustInherit Class Type

    '===============================
    '======== COMPILED NAME ========
    '===============================
    Public Overridable ReadOnly Property CompiledName As String
        Get
            Return Name
        End Get
    End Property

    '======================
    '======== NAME ========
    '======================
    Public ReadOnly Property Name As String

    '=========================
    '======== TYPE ID ========
    '=========================
    Public ReadOnly Property TypeID As Integer
    Private Shared TypesCount As Integer = 0

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New()

        ' Get type ID
        TypesCount += 1
        TypeID = TypesCount

        ' Create name
        Name = "C" & TypeID & "_t"

    End Sub

    '================================
    '======== CAN CONVERT TO ========
    '================================
    Public Overridable Function CanConvertTo(Request As Type) As Boolean
        Return False
    End Function

    '============================
    '======== CONVERT TO ========
    '============================
    Public Overridable Function ConvertTo(Expression As CExpression, Request As Type) As CExpression
        Return Nothing
    End Function


End Class