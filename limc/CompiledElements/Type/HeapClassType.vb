Public Class HeapClassType
    Inherits ClassType
    Implements IBuildableStructure
    Implements IGarbageCollectedType

    '=======================
    '======== CLASS ========
    '=======================
    Private FromClass As ClassConstructNode

    '===============================
    '======== COMPILED NAME ========
    '===============================
    Public Overrides ReadOnly Property CompiledName As String Implements IGarbageCollectedType.CompiledName
        Get
            Return MyBase.CompiledName & "*"
        End Get
    End Property

    '======================
    '======== NAME ========
    '======================
    Public Shadows ReadOnly Property Name As String Implements IGarbageCollectedType.Name
        Get
            Return MyBase.Name
        End Get
    End Property

    '=========================
    '======== DEFAULT ========
    '=========================
    Public Overrides ReadOnly Property DefaultValue As String = "NULL"

    '=============================
    '======== SOURCE NAME ========
    '=============================
    Protected Overrides ReadOnly Property SourceName As String
        Get
            Return FromClass.Name
        End Get
    End Property

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(From As ClassConstructNode, PassedGenericTypes As IEnumerable(Of Type))
        MyBase.New(From.Location.File)

        'Notify to file builder
        FileBuilder.NotifyNewStructure(Me)

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
        'Dim DefaultMethod As New CPrimitiveClassConstructor(Me, FromClass.DefaultState, True) 'if FromClass.DefaultState is nothing CConstructor will create a default method

        'Create assignement function
        CSourceFunction.GenerateSourceFunction(
            $"void {Name}_set({CompiledName}* variable, {CompiledName} value)",
            {
                "if (*variable != NULL)",
                vbTab & "(*variable)->stackReferences--;",
                "",
                "if (value != NULL)",
                vbTab & "value->stackReferences++;",
                "",
                "*variable = value;"
            }
        )

        'Create head for garbage collector
        GarbageCollector.AddType(Me)

        'Create mark function
        Dim MarkFunctionContent As New List(Of String) From {
            "if (self == NULL || self->marked)",
            vbTab & "return;",
            "",
            "self->marked = true;"
        }
        For Each Propertie As Propertie In Properties
            If TypeOf Propertie.Type Is IGarbageCollectedType Then
                MarkFunctionContent.Add(Propertie.Type.Name & "_mark(self->" & Propertie.CompiledName & ");")
            End If
        Next
        CSourceFunction.GenerateSourceFunction($"void {Name}_mark({CompiledName} self)", MarkFunctionContent)

        'Create free function
        CSourceFunction.GenerateSourceFunction($"void {Name}_free({CompiledName} self)", {
            "if (self == NULL)",
            vbTab & "return;",
            "free(self);"
        })

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
        Result.Add(vbTab & "unsigned long stackReferences;")
        Result.Add(vbTab & "bool marked;")
        Result.Add(vbTab & $"{CompiledName} next;")
        Result.Add("} " & Name & ";")

        'Return result
        Return Result

    End Function

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
            Return New CHeapClassConstructor(Me, UncompiledConstructor)
        End Get
    End Property

    '==============================
    '======== SET VARIABLE ========
    '==============================
    'Variable = NewValue
    Public Overrides Function SetVariable(Variable As String, NewValue As String) As String
        Return $"{Name}_set(&{Variable}, {NewValue});"
    End Function

End Class
