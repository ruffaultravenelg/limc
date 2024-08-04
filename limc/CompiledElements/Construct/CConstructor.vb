Public MustInherit Class CConstructor
    Implements BuildableFunction

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
    Public ReadOnly Property CompiledName As String
    Private Shared ConsructorCount As Integer = 0

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    'Node = Nothing when creating the default constructor
    Public Sub New(Type As ClassType, Node As MethodConstructNode, Optional DefaultMethod As Boolean = False)

        'Notice parent file that this function is compiled
        Type.NotifyNewCompiledConstructor(Me)

        'Notify ourself
        FileBuilder.NotifyNewFunction(Me)

        'Set parent type
        Me.ParentType = Type

        'Create name
        If DefaultMethod Then
            CompiledName = $"{ParentType.Name}__default"
        Else
            ConsructorCount += 1
            CompiledName = $"{ParentType.Name}__new{ConsructorCount.ToString()}"
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

    '=============================
    '======== CREATE SELF ========
    '=============================
    Protected MustOverride Function CreateSelf() As String

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
        Return $"{ParentType.CompiledName} {CompiledName}({Arguments})"

    End Function

    '=============================
    '======== BUILD LOGIC ========
    '=============================
    Private Function BuildLogic() As IEnumerable(Of String) Implements BuildableFunction.BuildLogic

        'Create result
        Dim Result As New List(Of String)

        'Create self
        Result.Add(CreateSelf())
        Result.Add("")

        'Init default states of variables
        For Each Propertie As Propertie In DirectCast(ParentType, ClassType).Properties
            Result.Add(Propertie.Setter(Propertie.Type.DefaultValue) & ";")
        Next
        Result.Add("")

        'Add constructor's logic
        Result.AddRange(CompiledLogic)
        Result.Add("")

        'Add return
        Result.Add("return self;")

        Return Result

    End Function

    '============================
    '======== LOOKS LIKE ========
    '============================
    Public Function LooksLike(ArgumentsTypes As IEnumerable(Of Type)) As Boolean

        'Passed arguments types count
        If Not Scope.Variables.Count = ArgumentsTypes.Count Then
            Return False
        End If

        'Passed arguments types
        For i As Integer = 0 To ArgumentsTypes.Count - 1
            If Not Scope.Variables(i).Type = ArgumentsTypes(i) Then 'TODO: Check type compatibility instead of equality
                Return False
            End If
        Next

        'Everything looks ok
        Return True

    End Function

End Class
