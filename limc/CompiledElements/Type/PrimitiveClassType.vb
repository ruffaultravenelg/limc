'
' Represent a primitive class type
'
Public Class PrimitiveClassType
    Inherits ClassType
    Implements IBuildableStructure

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

    '=====================================
    '======== BUILD: FORWARD TYPE ========
    '=====================================
    Private Function BuildTypeForward() As String Implements IBuildableStructure.BuildTypeForward
        Return $"typedef struct {Name} {Name};"
    End Function

    '=============================================
    '======== BUILD: STRUCTURE DEFINITION ========
    '=============================================
    Private Function BuildStructureDefinition() As IEnumerable(Of String) Implements IBuildableStructure.BuildStructureDefinition

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

    '=========================
    '======== DEFAULT ========
    '=========================
    Public Overrides ReadOnly Property DefaultValue As String = $"{Name}_default()"

    '====================================
    '======== UNCOMPILED METHODS ========
    '====================================
    Protected Overrides ReadOnly Property UncompiledMethods As IEnumerable(Of MethodConstructNode)
        Get
            Return FromClass.Methods
        End Get
    End Property

    '==============================
    '======== CONSTRUCTORS ========
    '==============================
    Protected Overrides ReadOnly Property UncompiledConstructors As IEnumerable(Of MethodConstructNode)
        Get
            Return FromClass.Constructors
        End Get
    End Property
    Protected Overrides ReadOnly Property CompiledConstructor(UncompiledConstructor As IUnCompiledProcedure) As CConstructor
        Get
            Return New CPrimitiveClassConstructor(Me, UncompiledConstructor)
        End Get
    End Property

    '==============================
    '======== SET VARIABLE ========
    '==============================
    'Variable = NewValue
    Public Overrides Function SetVariable(Variable As String, NewValue As String) As String
        Return Variable & " = " & NewValue & ";"
    End Function

End Class