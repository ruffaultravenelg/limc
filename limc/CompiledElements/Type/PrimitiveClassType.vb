'
' Represent a primitive class type
'
Public Class PrimitiveClassType
    Inherits Type
    Implements BuildableStructure, ClassType

    '===============================
    '======== EPONYME TYPES ========
    '===============================
    Private Shared ReadOnly PredefinedTypes As String() = {"int", "float", "bool"}

    '=============================
    '======== SOURCE NAME ========
    '=============================
    Protected Overrides ReadOnly Property SourceName As String
        Get
            Return FromClass.Name
        End Get
    End Property

    '=======================
    '======== CLASS ========
    '=======================
    Private FromClass As ClassConstructNode

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(From As ClassConstructNode, PassedGenericTypes As IEnumerable(Of Type))
        MyBase.New(From.Location.File, If(PredefinedTypes.Contains(From.Name), From.Name, Nothing))

        'Notify to file builder
        If Not PredefinedTypes.Contains(From.Name) Then
            FileBuilder.NotifyNewStructure(Me)
        End If

        'Set attached class of type's scope
        Me.Scope.AttachClass(Me)

        'Set class
        Me.FromClass = From

        'Set generic types
        Me.Scope.GenericTypes.AddRange(FromClass.CreateGenericTypes(PassedGenericTypes))

        'Create class's variables for the scope
        Dim PropertiesCounter As Integer = 0
        For Each DeclareVariable As DeclareVariableStatementNode In FromClass.DeclareVariables
            PropertiesCounter += 1
            Dim VariableCompiledName As String = $"propertie{PropertiesCounter}"
            If DeclareVariable.VariableType IsNot Nothing Then
                Properties.Add(New Propertie(Me, DeclareVariable.VariableName, DeclareVariable.VariableType.AssociatedType(Me.SharedScope), VariableCompiledName))
            Else
                Properties.Add(New Propertie(Me, DeclareVariable.VariableName, DeclareVariable.VariableValue.ReturnType(Me.SharedScope), VariableCompiledName))
            End If
        Next

        'Create default state
        Select Case FromClass.Name

            Case "int"
                DefaultValue = "0"

            Case "float"
                DefaultValue = "0.0"

            Case "bool"
                DefaultValue = "false"

            Case Else
                Dim DefaultMethod As New CPrimitiveClassConstructor(Me, FromClass.DefaultState, True) 'if FromClass.DefaultState is nothing CConstructor will create a default method

        End Select

    End Sub

    '============================
    '======== PROPERTIES ========
    '============================
    Private ReadOnly Property Properties As New List(Of Propertie) Implements ClassType.Properties

    '=====================================
    '======== BUILD: FORWARD TYPE ========
    '=====================================
    Private Function BuildTypeForward() As String Implements BuildableStructure.BuildTypeForward
        Return $"typedef struct {Name} {Name};"
    End Function

    '=============================================
    '======== BUILD: STRUCTURE DEFINITION ========
    '=============================================
    Private Function BuildStructureDefinition() As IEnumerable(Of String) Implements BuildableStructure.BuildStructureDefinition

        'Create result
        Dim Result As New List(Of String)

        'Header
        Result.Add("// " & ToString())
        Result.Add("typedef struct " & Name & "{")

        'All propertie
        For Each Propertie As Propertie In Properties
            Result.Add(vbTab & $"{Propertie.Type.CompiledName} {Propertie.CompiledName}; //{Propertie.Name}")
        Next

        'Footer
        Result.Add("} " & Name & ";")

        'Return result
        Return Result

    End Function

    '==============================
    '======== CONSTRUCTORS ========
    '==============================
    Public ReadOnly Property Constructor(ArgumentsTypes As IEnumerable(Of Type)) As CConstructor Implements ClassType.Constructor
        Get

            'Search if there is a compiled getter that correspond
            For Each CompiledConstructor As CConstructor In CompiledConstructors
                If CompiledConstructor.LooksLike(ArgumentsTypes) Then
                    Return CompiledConstructor
                End If
            Next

            'Search if there is a non-compiled constructor that correspond
            For Each ConstructorMethod As MethodConstructNode In Me.FromClass.Constructors
                If ConstructorMethod.CouldBe("new", {}, ArgumentsTypes) Then
                    Return New CPrimitiveClassConstructor(Me, ConstructorMethod)
                End If
            Next

            'Not found
            Throw New ClassType.ConstructorNotFoundException(Me.ToString())

        End Get
    End Property

    'List of all already compiled constructors
    Private CompiledConstructors As New List(Of CConstructor)

    'Notify
    Private Sub NotifyNewCompiledConstructor(Constructor As CConstructor) Implements ClassType.NotifyNewCompiledConstructor
        Me.CompiledConstructors.Add(Constructor)
    End Sub

    '=========================
    '======== DEFAULT ========
    '=========================
    Public Overrides ReadOnly Property DefaultValue As String = $"{Name}__default()"

    '=========================
    '======== DEFAULT ========
    '=========================
    Protected Overrides Function SearchMethod(Name As String, GenericTypes As IEnumerable(Of Type), ArgumentTypes As IEnumerable(Of Type)) As CMethod

        For Each Method As MethodConstructNode In FromClass.Methods
            If Method.CouldBe(Name, GenericTypes, ArgumentTypes) Then
                Return New CMethod(Me, Method, GenericTypes)
            End If
        Next

        Return Nothing

    End Function

End Class