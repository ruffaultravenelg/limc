Public Class FunctionConstructNode
    Inherits LogicContainerConstruct

    '===============================
    '======== FUNCTION NAME ========
    '===============================
    Public ReadOnly Name As String

    '===============================
    '======== GENERIC TYPES ========
    '===============================
    Private ReadOnly GenericTypes As List(Of GenericTypeNode)

    '===========================
    '======== ARGUMENTS ========
    '===========================
    Private ReadOnly Arguments As List(Of FunctionArgumentNode)

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Public ReadOnly ReturnType As TypeNode

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, Logic As List(Of StatementNode), Name As String, GenericTypes As List(Of GenericTypeNode), Arguments As List(Of FunctionArgumentNode), Optional ReturnType As TypeNode = Nothing)
        MyBase.New(Location, Logic)

        'Set properties
        Me.Name = Name
        Me.GenericTypes = GenericTypes
        Me.Arguments = Arguments
        Me.ReturnType = ReturnType

    End Sub

    '========================================
    '======== GENERATE GENERIC TYPES ========
    '========================================
    Public Sub GenerateGenericTypes(Scope As Scope, GenericTypes As IEnumerable(Of Type))
        For i As Integer = 0 To Me.GenericTypes.Count - 1
            Scope.GenericTypes.Add(New GenericType(Me.GenericTypes(i).Name, GenericTypes(i)))
        Next
    End Sub

    '==============================================
    '======== GENERATE ARGUMENTS VARIABLES ========
    '==============================================
    Public Sub GenerateArgumentsVariables(Target As Scope)
        For i As Integer = 0 To Me.Arguments.Count - 1
            Target.NotifyVariableExist(New Variable(Me.Arguments(i).Name, Arguments(i).Type.AssociatedType(Target)))
        Next
    End Sub

    '==========================
    '======== COULD BE ========
    '==========================
    Public Function CouldBe(Name As String, PassedGenericTypes As IEnumerable(Of Type), ArgumentsTypes As IEnumerable(Of Type)) As Boolean

        'Name
        If Not Me.Name = Name Then
            Return False
        End If

        'Passed generic types count
        If Not Me.GenericTypes.Count = PassedGenericTypes.Count Then
            Return False
        End If

        'Passed generic types
        For i As Integer = 0 To PassedGenericTypes.Count - 1
            If Not Me.GenericTypes(i).IsComptatible(PassedGenericTypes(i)) Then
                Return False
            End If
        Next

        'Passed arguments types count
        If Not Arguments.Count = ArgumentsTypes.Count Then
            Return False
        End If

        'Passed arguments types
        Dim FunctionScope As New Scope(Me.Location.File.Scope)
        GenerateGenericTypes(FunctionScope, PassedGenericTypes)
        For i As Integer = 0 To ArgumentsTypes.Count - 1

            Dim WantedArgumentType As Type = Me.Arguments(i).Type.AssociatedType(FunctionScope)
            Dim PassedArgumentType As Type = ArgumentsTypes(i)

            If Not PassedArgumentType = WantedArgumentType Then 'TODO: Check type compatibility instead of equality
                Return False
            End If
        Next

        'Everything looks ok
        Return True

    End Function
    Public Function CouldBe(Name As String, PassedGenericTypes As IEnumerable(Of Type)) As Boolean

        'Name
        If Not Me.Name = Name Then
            Return False
        End If

        'Passed generic types count
        If Not Me.GenericTypes.Count = PassedGenericTypes.Count Then
            Return False
        End If

        'Passed generic types
        For i As Integer = 0 To PassedGenericTypes.Count - 1
            If Not Me.GenericTypes(i).IsComptatible(PassedGenericTypes(i)) Then
                Return False
            End If
        Next

        'Everything looks ok
        Return True

    End Function

End Class
