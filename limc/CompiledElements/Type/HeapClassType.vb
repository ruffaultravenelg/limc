Public Class HeapClassType
    Inherits ClassType

    '=============================
    '======== SOURCE NAME ========
    '=============================
    Protected Overrides ReadOnly Property SourceName As String
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Owner As LimSource)
        MyBase.New(Owner)
    End Sub

    '===============================
    '======== COMPILED NAME ========
    '===============================
    Public Overrides ReadOnly Property CompiledName As String
        Get
            Return MyBase.CompiledName & "*"
        End Get
    End Property

    '=========================
    '======== DEFAULT ========
    '=========================
    Public Overrides ReadOnly Property DefaultValue As String = "NULL"

    '==============================
    '======== CONSTRUCTORS ========
    '==============================
    Protected Overrides ReadOnly Property UncompiledConstructors As IEnumerable(Of MethodConstructNode)
        Get
            Return Nothing
        End Get
    End Property
    Protected Overrides ReadOnly Property CompiledConstructor(UncompiledConstructor As IUnCompiledProcedure) As CConstructor
        Get
            Return New CPrimitiveClassConstructor(Me, UncompiledConstructor)
        End Get
    End Property


End Class
