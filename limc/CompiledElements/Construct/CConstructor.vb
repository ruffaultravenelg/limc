Public MustInherit Class CConstructor
    Implements IBuildableFunction
    Implements ICompiledProcedure

    '======================
    '======== NAME ========
    '======================
    Private ReadOnly Property Name As String = "new" Implements ICompiledProcedure.Name

    '===============================
    '======== GENERIC TYPES ========
    '===============================
    Private ReadOnly Property GenericTypes As IEnumerable(Of Type) = {} Implements ICompiledProcedure.GenericTypes

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

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Private ReadOnly Property ReturnType As Type = Nothing Implements ICompiledProcedure.ReturnType

    '========================
    '======== TARGET ========
    '========================
    Protected ParentType As Type

    '===================================
    '======== CONSTRUCTOR SCOPE ========
    '===================================
    Private Scope As Scope 'Only for arguments, use deeper scope for content

    '===============================
    '======== COMPIED LOGIC ========
    '===============================
    Private CompiledLogic As IEnumerable(Of String)

    '===============================
    '======== COMPILED NAME ========
    '===============================
    Public ReadOnly Property CompiledName As String Implements ICompiledProcedure.CompiledName
    Private Shared ConsructorCount As Integer = 0

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    'Node = Nothing when creating the default constructor
    Public Sub New(Type As ClassType, Node As MethodConstructNode, Optional DefaultMethod As Boolean = False)

        'Notice parent file that this function is compiled
        If Not DefaultMethod Then
            Type.NotifyNewCompiledConstructor(Me)
        End If

        'Notify ourself
        FileBuilder.NotifyNewFunction(Me)

        'Set parent type
        Me.ParentType = Type

        'Create name
        If DefaultMethod Then
            CompiledName = $"{ParentType.Name}_default"
        Else
            ConsructorCount += 1
            CompiledName = $"{ParentType.Name}_new{ConsructorCount.ToString()}"
        End If

        'Create scope
        Me.Scope = New Scope(ParentType.Scope)

        'Get arguments varaibles (here for looksLike())
        If Node IsNot Nothing Then
            Node.GenerateArgumentsVariables(Scope)
        End If

        'Content scope
        Dim ContentScope As New Scope(Scope)

        'Compile content
        If Node IsNot Nothing Then
            For Each Statement As StatementNode In Node.Logic
                Statement.Compile(ContentScope)
            Next
        End If

        'Get logic
        CompiledLogic = ContentScope.Result

    End Sub

    '===================================
    '======== SUBCLASS COMPLETE ========
    '===================================
    Protected MustOverride Function CreateSelf() As IEnumerable(Of String)
    Protected MustOverride Function ReturnValue() As String

    '=================================
    '======== BUILD PROTOTYPE ========
    '=================================
    Private Function BuildPrototype() As String Implements IBuildableFunction.BuildPrototypeWithoutSemiColon

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
        Return $"{ParentType.CompiledName} {CompiledName}({Arguments})"

    End Function

    '=============================
    '======== BUILD LOGIC ========
    '=============================
    Private Function BuildLogic() As IEnumerable(Of String) Implements IBuildableFunction.BuildLogic

        'Create result
        Dim Result As New List(Of String)

        'Create self
        Result.AddRange(CreateSelf())
        Result.Add("")

        'Init default states of variables
        For Each Propertie As Propertie In DirectCast(ParentType, ClassType).Properties
            Result.Add(Propertie.Type.SetVariable(Propertie.AcessName, Propertie.Type.DefaultValue))
        Next
        Result.Add("")

        'Add constructor's logic
        Result.AddRange(CompiledLogic)
        Result.Add("")

        'Add return
        Result.Add($"return {ReturnValue()};")

        Return Result

    End Function

End Class
