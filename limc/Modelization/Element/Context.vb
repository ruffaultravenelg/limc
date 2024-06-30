Public MustInherit Class Context

    '===================================
    '======== CONTEXT VARIABLES ========
    '===================================
    Private ReadOnly Property Variables As New List(Of Variable)

    '=======================================
    '======== CONTEXT GENERIC TYPES ========
    '=======================================
    Private ReadOnly Property GenericTypes As New List(Of GenericType)

    '=========================================
    '======== EXCEPETION HANDLER NAME ========
    '=========================================
    Private ReadOnly Property ExceptionHandlerName As String

    '================================
    '======== PARENT CONTEXT ========
    '================================
    Private ReadOnly Parent As Context

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Optional Parent As Context = Nothing)
        Me.Parent = Parent
    End Sub

    '=========================
    '======== COMPILE ========
    '=========================
    ' Compile is called only when Nodes finish compiling
    Public Sub Compile(Result As List(Of String))

        'Create all variables
        CreateVariable(Result)

        'Compile content
        Assemble(Result)

        'Remove reference from result
        RemoveReferenceVariable(Result)

    End Sub

    'Create each variables used in the context
    Private Sub CreateVariable(Result As List(Of String))

        'Define each variable
        For Each Variable As Variable In Variables
            Result.Add($"{Variable.Type.CompiledName} {Variable.CompiledName};")
        Next

    End Sub

    'Remove reference for each used variable in the context
    Private Sub RemoveReferenceVariable(Result As List(Of String))

        'Remove a reference to each heap variables
        For Each Variable As Variable In Variables

            'If variable is not heap goto next variable
            If TypeOf Variable.Type IsNot HeapType Then
                Continue For
            End If

            'Remove a reference if variable is not null
            DirectCast(Variable.Type, HeapType).RemoveReference(Result, Variable.CompiledName)

        Next

    End Sub

    '==========================
    '======== ASSEMBLE ========
    '==========================
    Protected MustOverride Sub Assemble(Result As List(Of String))

End Class
