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

    '==================================
    '======== GET LIST OF TYPE ========
    '==================================
    Public Shared Function GetTypes(GenericTypes As IEnumerable(Of GenericType)) As List(Of Type)
        Dim Result As New List(Of Type)
        For Each GenericType As GenericType In GenericTypes
            Result.Add(GenericType.Type)
        Next
        Return Result
    End Function

End Class
