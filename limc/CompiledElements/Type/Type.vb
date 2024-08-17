'
' Represent a Type
'

Public MustInherit Class Type

    '==============================
    '======== COMMON TYPES ========
    '==============================
    Public Shared int As PrimitiveClassType
    Public Shared bool As PrimitiveClassType

    '============================
    '======== TYPE SCOPE ========
    '============================
    Public ReadOnly Property Scope As Scope = New Scope()
    Public ReadOnly Property SharedScope As Scope
        Get
            Dim Result As New Scope(OwnerFile.Scope)
            Result.GenericTypes.AddRange(Scope.GenericTypes)
            Return Result
        End Get
    End Property

    '===============================
    '======== COMPILED NAME ========
    '===============================
    Public Overridable ReadOnly Property CompiledName As String
        Get
            Return Name
        End Get
    End Property

    '======================
    '======== NAME ========
    '======================
    Public ReadOnly Property Name As String

    '======================
    '======== NAME ========
    '======================
    Protected MustOverride ReadOnly Property SourceName As String

    '=========================
    '======== DEFAULT ========
    '=========================
    Public MustOverride ReadOnly Property DefaultValue As String

    '============================
    '======== OWNER FILE ========
    '============================
    Private OwnerFile As LimSource

    '=========================
    '======== TYPE ID ========
    '=========================
    Public ReadOnly Property TypeID As Integer
    Private Shared TypesCount As Integer = 0

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(File As LimSource, Optional CustomCompiledName As String = Nothing)

        'Owner file
        OwnerFile = File
        OwnerFile.NoticeNewCompiledType(Me)

        ' Get type ID
        TypesCount += 1
        TypeID = TypesCount

        ' Create name
        If CustomCompiledName Is Nothing Then
            Name = "C" & TypeID & "_t"
        Else
            Name = CustomCompiledName
        End If

    End Sub

    '=======================
    '======== EQUAL ========
    '=======================
    Public Shared Operator =(A As Type, B As Type) As Boolean
        If (A Is Nothing AndAlso B Is Nothing) Then
            Return True
        End If
        If (A Is Nothing OrElse B Is Nothing) Then
            Return False
        End If
        Return A.TypeID = B.TypeID
    End Operator
    Public Shared Operator <>(A As Type, B As Type) As Boolean
        Return Not (A = B)
    End Operator

    '================================
    '======== CAN CONVERT TO ========
    '================================
    Public Overridable Function CanConvertTo(Request As Type) As Boolean

        'Same type
        If Me = Request Then
            Return True
        End If

        ' Cannot
        Return False

    End Function

    '============================
    '======== CONVERT TO ========
    '============================
    Public Overridable Function ConvertTo(Expression As String, Request As Type) As String

        'Same type
        If Me = Request Then
            Return Expression
        End If

        'Search for @converter
        Throw New NotImplementedException

    End Function

    '========================================
    '======== STRINGIFY LIST OF TYPE ========
    '========================================
    Public Shared Function StringifyListOfType(Types As IEnumerable(Of Type)) As String

        'No types
        If Types.Count = 0 Then
            Return "<>"
        End If

        'At least one type
        Dim Result As String = ""
        For Each Type As Type In Types
            Result &= ", " & Type.ToString()
        Next
        Return "<" & Result.Substring(2) & ">"

    End Function

    '===========================
    '======== TO STRING ========
    '===========================
    Public Overrides Function ToString() As String

        'Is there somes generic types
        If SharedScope.GenericTypes.Count > 0 Then
            Return SourceName & limc.Type.StringifyListOfType(GenericType.GetTypes(SharedScope.GenericTypes))
        Else
            Return SourceName
        End If

    End Function

    '============================
    '======== LOOKS LIKE ========
    '============================
    Public Function LooksLike(Name As String, PassedGenericTypes As IEnumerable(Of Type)) As Boolean

        'Not the same name
        If Not Name = SourceName Then
            Return False
        End If

        'Not the same generic argument count
        If Not PassedGenericTypes.Count = SharedScope.GenericTypes.Count Then
            Return False
        End If

        'Not the same arguments
        For i As Integer = 0 To PassedGenericTypes.Count - 1
            If Not SharedScope.GenericTypes(i).Type = PassedGenericTypes(i) Then
                Return False
            End If
        Next

        'Corporate says there are the same
        Return True

    End Function

    '========================
    '======== METHOD ========
    '========================
    Public ReadOnly Property Method(Name As String, GenericTypes As IEnumerable(Of Type), ArgumentTypes As IEnumerable(Of Type)) As CMethod
        Get

            'Search if there is a compiled method that correspond
            For Each CompiledMethod As CMethod In CompiledMethods
                If CompiledMethod.LooksLike(Name, GenericTypes, ArgumentTypes) Then
                    Return CompiledMethod
                End If
            Next

            'Search if there is a non-compiled method that correspond
            Dim NonCompiledMethod As CMethod = SearchMethod(Name, GenericTypes, ArgumentTypes)
            If NonCompiledMethod IsNot Nothing Then
                Return NonCompiledMethod
            End If

            'Not found
            Throw New MethodNotFoundException(Name)

        End Get
    End Property

    'List of all already compiled methods
    Private CompiledMethods As New List(Of CMethod)

    'Notify the type that a new method has be compiled
    Public Sub NotifyNewCompiledMethod(Method As CMethod)
        Me.CompiledMethods.Add(Method)
    End Sub

    'Allow for sub-classes to indicate non-compiled methods
    Protected Overridable Function SearchMethod(Name As String, GenericTypes As IEnumerable(Of Type), ArgumentTypes As IEnumerable(Of Type)) As CMethod
        Return Nothing
    End Function

    'When a getter is not found
    Public Class MethodNotFoundException
        Inherits CompilerException
        Public Sub New(Name As String)
            MyBase.New("Method not found", $"Method ""{Name}"" not found.")
        End Sub
    End Class

End Class