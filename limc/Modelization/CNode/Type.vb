'
' Represent a Type
'
Public MustInherit Class Type

    '==============================
    '======== COMMON TYPES ========
    '==============================
    Public Shared ReadOnly int As Type = Nothing
    Public Shared ReadOnly float As Type = Nothing
    Public Shared ReadOnly str As Type = Nothing
    Public Shared ReadOnly bool As Type = Nothing

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

    '========================
    '======== GETTER ========
    '========================
    Public ReadOnly Property Getter(Name As String) As CGetter
        Get

            'Search if there is a compiled getter that correspond
            For Each CompiledGetter As CGetter In CompiledGetters
                If CompiledGetter.Correspond(Name) Then
                    Return CompiledGetter
                End If
            Next

            'Search if there is a non-compiled getter that correspond
            Return SearchGetter(Name)

        End Get
    End Property

    'List of all already compiled getters
    Private CompiledGetters As New List(Of CGetter)

    'Allow for sub-classes to indicate non-compiled Getter
    Protected Overridable Function SearchGetter(Name As String) As CGetter
        Throw New GetterNotFoundException(Name)
    End Function

    'When a getter is not found
    Public Class GetterNotFoundException
        Inherits CompilerException
        Public Sub New(Name As String)
            MyBase.New("Getter not found", $"Getter ""{Name}"" not found")
        End Sub
    End Class

End Class