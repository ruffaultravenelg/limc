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

    '============================================================
    '======== FUNCTION RETURN TYPE (only for `fun` type) ========
    '============================================================
    Private ReadOnly Property FunctionReturnType As TypeNode

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, Name As String, PassedGenericTypes As List(Of TypeNode), FunctionReturnType As TypeNode)
        MyBase.New(Location)

        'Set name
        Me.Name = Name

        'Set passed generic types
        Me.PassedGenericTypes = PassedGenericTypes

        'Set function return type
        Me.FunctionReturnType = FunctionReturnType

    End Sub

    '=================================
    '======== ASSOCIATED TYPE ========
    '=================================
    Public Function AssociatedType(Scope As Scope) As Type

        'Check for generic type
        If Me.PassedGenericTypes.Count = 0 AndAlso FunctionReturnType Is Nothing Then
            For Each Parent As Scope In Scope.Uppers
                For Each GenericType As GenericType In Parent.GenericTypes
                    If Name = GenericType.Name Then
                        Return GenericType.Type
                    End If
                Next
            Next
        End If

        'Convert passed generic types into types
        Dim GenericTypes As New List(Of Type)
        For Each TypeNode As TypeNode In PassedGenericTypes
            GenericTypes.Add(TypeNode.AssociatedType(Scope))
        Next

        'Function return type
        If Name = "fun" Then

            'Get function return type
            Dim FunctionReturnTrueType As Type = If(FunctionReturnType Is Nothing, Nothing, FunctionReturnType.AssociatedType(Scope))

            'Get this type
            Return FunctionPointers.FromTypes(GenericTypes, FunctionReturnTrueType)

        End If

        'Search type in current file
        Try
            Return Me.Location.File.Type(Name, GenericTypes)
        Catch ex As LimSource.CannotFindTypeException
            Throw New LocalizedException("Type not found", $"Unable to find ""{ToString()}"" type from ""{Me.Location.File.RelativePath}"" file. Check its name, generic types and visibility.", Me.Location)
        End Try

    End Function

    '===========================
    '======== TO STRING ========
    '===========================
    Public Overrides Function ToString() As String

        If Me.PassedGenericTypes.Count > 0 Then

            Dim Arguments_STR As String = ""
            For Each GenericType As TypeNode In PassedGenericTypes
                Arguments_STR &= ", " & GenericType.ToString()
            Next

            Return $"{Name}<{Arguments_STR.Substring(2)}>"

        Else

            Return Name

        End If

    End Function

End Class
