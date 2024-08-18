Imports System.Diagnostics.CodeAnalysis

Public Class CFunction
    Implements BuildableFunction
    Implements Procedure.CompiledProcedure

    '================================
    '======== FUNCTION SCOPE ========
    '================================
    Private Node As FunctionConstructNode
    Private ReadOnly Property Name As String Implements CompiledProcedure.Name
        Get
            Return Node.Name
        End Get
    End Property

    '================================
    '======== FUNCTION SCOPE ========
    '================================
    Private Scope As Scope 'Only for arguments, use deeper scope for content
    Private ReadOnly Property GenericTypes As IEnumerable(Of Type) Implements CompiledProcedure.GenericTypes
        Get
            Dim Result As New List(Of Type)
            For Each GT As GenericType In Scope.GenericTypes
                Result.Add(GT.Type)
            Next
            Return Result
        End Get
    End Property

    '===========================
    '======== ARGUMENTS ========
    '===========================
    Private ReadOnly Property ArgumentsTypes As IEnumerable(Of Type) Implements CompiledProcedure.Arguments
        Get
            Dim Result As New List(Of Type)
            For Each Var As Variable In Scope.Variables
                Result.Add(Var.Type)
            Next
            Return Result
        End Get
    End Property

    '================================
    '======== COMPILED LOGIC ========
    '================================
    Private CompiledLogic As IEnumerable(Of String)

    '===============================
    '======== COMPILED NAME ========
    '===============================
    Public ReadOnly Property CompiledName As String Implements CompiledProcedure.CompiledName
    Private Shared FunctionCount As Integer = 0

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Private _ReturnType As Type = Nothing
    Public ReadOnly Property ReturnType As Type Implements CompiledProcedure.ReturnType
        Get

            'There is a return type
            If _ReturnType IsNot Nothing Then
                Return _ReturnType
            End If

            'There is no return type and the function has no return -> no return type = nothing
            If Not Node.HasReturn Then
                Return Nothing
            End If

            'No returntype wet
            Throw New SyntaxErrorException("The function's return type is requested before the compiler can find it. Please specify the return type explicitly.", Node.Location)

        End Get
    End Property
    Private Sub SetReturnType(NewType As Type, Location As Location)

        'No type registered
        If _ReturnType Is Nothing Then
            _ReturnType = NewType
            Exit Sub
        End If

        'Type registered -> compare type
        If Not _ReturnType = NewType Then
            Throw New LocalizedException("The return type is not the same", "The return type of the function has been defined as """ & _ReturnType.ToString() & """, but here you're trying to return a """ & NewType.ToString() & """ value.", Location)
        End If

    End Sub

    '================================
    '======== SIGNATURE TYPE ========
    '================================
    Private _SignatureType As FunctionSignatureType = Nothing
    Public ReadOnly Property SignatureType As FunctionSignatureType
        Get

            'There is a return type
            If _SignatureType IsNot Nothing Then
                Return _SignatureType
            End If

            'Get argument types
            Dim ArgumentsTypes As New List(Of Type)
            For Each Variable As Variable In Scope.Variables
                ArgumentsTypes.Add(Variable.Type)
            Next

            'Create and compile signature type
            _SignatureType = FunctionPointers.FromTypes(ArgumentsTypes, ReturnType)
            Return _SignatureType

        End Get
    End Property

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Node As FunctionConstructNode, GenericTypes As IEnumerable(Of Type))

        'Notice parent file that this function is compiled
        Node.Location.File.NoticeNewCompiledFunction(Me)

        'Notify ourself
        FileBuilder.NotifyNewFunction(Me)

        'Set node
        Me.Node = Node

        'Create name
        CFunction.FunctionCount += 1
        CompiledName = $"Function{CFunction.FunctionCount.ToString()}"

        'Create scope
        Me.Scope = New Scope(Me.Node.Location.File.Scope, AddressOf SetReturnType)

        'Add generic type
        Node.AddGenericTypeToScope(Scope, GenericTypes)

        'Get arguments varaibles (here for looksLike())
        Node.GenerateArgumentsVariables(Scope)

        'Compile explicit return type
        If Node.ReturnType IsNot Nothing Then
            _ReturnType = Node.ReturnType.AssociatedType(Scope)
        End If

        'Function content
        Dim ContentScope As New Scope(Scope)
        For Each Statement As StatementNode In Me.Node.Logic
            Statement.Compile(ContentScope)
        Next
        CompiledLogic = ContentScope.Result

    End Sub

    '=================================
    '======== BUILD PROTOTYPE ========
    '=================================
    Private Function BuildPrototype() As String Implements BuildableFunction.BuildPrototypeWithoutSemiColon

        'Compile arguments signatures
        Dim Arguments As String = ""
        For Each Variable As Variable In Scope.Variables
            Arguments &= $", {Variable.Type.CompiledName} {Variable.CompiledName}"
        Next
        If Arguments.StartsWith(", ") Then
            Arguments = Arguments.Substring(2)
        Else
            Arguments = "void"
        End If

        'Assemble and return result
        If ReturnType Is Nothing Then
            Return $"void {CompiledName}({Arguments})"
        Else
            Return $"{ReturnType.CompiledName} {CompiledName}({Arguments})"
        End If

    End Function

    '=============================
    '======== BUILD LOGIC ========
    '=============================
    Private Function BuildLogic() As IEnumerable(Of String) Implements BuildableFunction.BuildLogic
        Return CompiledLogic
    End Function

End Class
