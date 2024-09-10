Public Class CMethod
    Implements IBuildableFunction
    Implements Procedure.ICompiledProcedure

    '======================
    '======== NAME ========
    '======================
    Private ReadOnly Property Name As String Implements ICompiledProcedure.Name
        Get
            Return Node.Name
        End Get
    End Property

    '===============================
    '======== GENERIC TYPES ========
    '===============================
    Private ReadOnly Property GenericTypes As IEnumerable(Of Type) Implements ICompiledProcedure.GenericTypes
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
    Private ReadOnly Property ArgumentsTypes As IEnumerable(Of Type) Implements ICompiledProcedure.Arguments
        Get
            Dim Result As New List(Of Type)
            For Each Var As Variable In Scope.Variables
                Result.Add(Var.Type)
            Next
            Return Result
        End Get
    End Property

    '================================
    '======== FUNCTION SCOPE ========
    '================================
    Private Node As MethodConstructNode

    '==============================
    '======== METHOD SCOPE ========
    '==============================
    Private Scope As Scope 'Only for arguments, use deeper scope for content

    '===============================
    '======== COMPIED LOGIC ========
    '===============================
    Private CompiledLogic As IEnumerable(Of String)

    '===============================
    '======== COMPILED NAME ========
    '===============================
    Public ReadOnly Property CompiledName As String Implements ICompiledProcedure.CompiledName
    Private Shared MethodCount As Integer = 0

    '==============================
    '======== PARENT CLASS ========
    '==============================
    Private ParentClass As ClassType

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Private _ReturnType As Type = Nothing
    Public ReadOnly Property ReturnType As Type Implements ICompiledProcedure.ReturnType
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
    Public Sub New(ParentClass As ClassType, Node As MethodConstructNode, GenericTypes As IEnumerable(Of Type))

        'Notice parent file that this function is compiled
        ParentClass.NotifyNewCompiledMethod(Me)

        'Notify ourself
        FileBuilder.NotifyNewFunction(Me)

        'Set node
        Me.Node = Node

        'Set parent class
        Me.ParentClass = ParentClass

        'Create name
        MethodCount += 1
        CompiledName = $"{ParentClass.Name}_Method{MethodCount.ToString()}"

        'Create scope
        Me.Scope = New Scope(ParentClass.Scope, AddressOf SetReturnType)

        'Add generic type
        Node.AddGenericTypeToScope(Scope, GenericTypes)

        'Get arguments varaibles (here for looksLike())
        Node.GenerateArgumentsVariables(Scope)

        'Compile explicit return type
        If Node.ReturnType IsNot Nothing Then
            _ReturnType = Node.ReturnType.AssociatedType(Scope)
        End If

        'Create content scope
        Dim ContentScope As New Scope(Scope)

        'Check null self
        ContentScope.WriteLine("if (self == NULL) lim_panic(""The \""" & Node.Name & "\"" method has been called on a null object of type \""" & ParentClass.ToString() & "\""."");")
        ContentScope.WriteLine("")

        'Compile method's statements
        For Each Statement As StatementNode In Me.Node.Logic
            Statement.Compile(ContentScope)
        Next

        'Set logic
        CompiledLogic = ContentScope.Result

    End Sub

    '=================================
    '======== BUILD PROTOTYPE ========
    '=================================
    Private Function BuildPrototype() As String Implements IBuildableFunction.BuildPrototypeWithoutSemiColon

        'Compile arguments signatures
        Dim Arguments As String = ""
        For Each Variable As Variable In Scope.Variables
            Arguments &= $", {Variable.Type.CompiledName} {Variable.CompiledName}"
        Next

        'Assemble and return result
        If ReturnType Is Nothing Then
            Return $"void {CompiledName}(void* self{Arguments})"
        Else
            Return $"{ReturnType.CompiledName} {CompiledName}(void* self{Arguments})"
        End If

    End Function

    '=============================
    '======== BUILD LOGIC ========
    '=============================
    Private Function BuildLogic() As IEnumerable(Of String) Implements IBuildableFunction.BuildLogic
        Return CompiledLogic
    End Function

End Class
