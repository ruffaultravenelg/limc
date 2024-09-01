Public Class MethodConstructNode
    Inherits LogicContainerConstruct
    Implements IUnCompiledProcedure

    '===============================
    '======== FUNCTION NAME ========
    '===============================
    Public ReadOnly Property Name As String Implements IUnCompiledProcedure.Name

    '===============================
    '======== GENERIC TYPES ========
    '===============================
    Private ReadOnly Property GenericTypes As List(Of GenericTypeNode) Implements IUnCompiledProcedure.GenericTypes

    '===========================
    '======== ARGUMENTS ========
    '===========================
    Private ReadOnly Property Arguments As List(Of FunctionArgumentNode) Implements IUnCompiledProcedure.Arguments

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Public ReadOnly ReturnType As TypeNode

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, Logic As List(Of StatementNode), Name As String, GenericTypes As List(Of GenericTypeNode), Arguments As List(Of FunctionArgumentNode), ReturnType As TypeNode)
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
    Private Function GenerateGenericType(GenericTypes As IEnumerable(Of Type), ParentClass As Scope) As Scope Implements IUnCompiledProcedure.GenerateScope

        'Create scope
        Dim Scope As New Scope(ParentClass)

        'Add generic type
        AddGenericTypeToScope(Scope, GenericTypes)

        'Return scope
        Return Scope

    End Function
    Public Sub AddGenericTypeToScope(Scope As Scope, GenericTypes As IEnumerable(Of Type))
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

End Class
