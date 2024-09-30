Public MustInherit Class ClassType
    Inherits Type

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Owner As LimSource)
        MyBase.New(Owner)
    End Sub
    Public Sub New(Owner As LimSource, CustomCompiledName As String)
        MyBase.New(Owner, CustomCompiledName)
    End Sub

    '==============================
    '======== CONSTRUCTORS ========
    '==============================
    Private CompiledConstructors As New List(Of CConstructor)
    ReadOnly Property Constructor(ArgumentTypes As IEnumerable(Of Type)) As CConstructor
        Get

            'Search best
            Try
                Return Procedure.SearchBestProcedure(CompiledConstructors, UncompiledConstructors, "new", {}, Function(x, y) CompiledConstructor(x), Me.Scope)
            Catch ex As SearchProcedureException
            End Try

            'Default constructor?
            If ArgumentTypes.Count = 0 AndAlso UncompiledConstructors.Count = 0 AndAlso TypeOf Me IsNot ArrayType Then
                Return CompiledConstructor(Nothing) 'Nothing => Default constructor
            End If

            'Not found
            Throw New ClassType.ConstructorNotFoundException(Me.ToString())

        End Get
    End Property
    Public Sub NotifyNewCompiledConstructor(Constructor As CConstructor)
        CompiledConstructors.Add(Constructor)
    End Sub

    'Allow for sub-classes to indicate non-compiled constructors
    Protected Overridable ReadOnly Property UncompiledConstructors As IEnumerable(Of MethodConstructNode) = New List(Of MethodConstructNode)

    'Compile constructor
    Protected MustOverride ReadOnly Property CompiledConstructor(UncompiledConstructor As IUnCompiledProcedure) As CConstructor

    '============================
    '======== PROPERTIES ========
    '============================
    Public ReadOnly Property Properties As New List(Of Propertie)

    'When a getter is not found
    Class ConstructorNotFoundException
        Inherits CompilerException
        Public Sub New(Type As String)
            MyBase.New("Constructor not found", $"The ""{Type}"" type does not contain an adapter constructor.")
        End Sub
    End Class

End Class
