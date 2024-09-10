Public Class ArrayType
    Inherits ClassType
    Implements IBuildableStructure
    Implements IGarbageCollectedType

    '=============================
    '======== TYPE LISTED ========
    '=============================
    Private TypeListed As Type

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
    Protected Overrides ReadOnly Property SourceName As String = "array"

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Private Sub New(ListedType As Type)
        MyBase.New(LimSource.STD)

        'Set listed type
        Me.TypeListed = ListedType

        'Notify to file builder
        FileBuilder.NotifyNewStructure(Me)

        'Set attached class of type's scope
        Me.Scope.AttachClass(Me)

        'Set generic types
        Me.Scope.GenericTypes.Add(New GenericType("T", ListedType))

        'Create constructor
        Dim Constructor As New CArrayConstructor(Me)

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
        If TypeOf ListedType Is IGarbageCollectedType Then
            MarkFunctionContent.Add("")
            MarkFunctionContent.Add("for (int i = 0; i < self->length; i++)")
            MarkFunctionContent.Add(vbTab & ListedType.Name & "_mark(self->array[i]);")
        End If
        CSourceFunction.GenerateSourceFunction($"void {Name}_mark({CompiledName} self)", MarkFunctionContent)

        'Create free function
        CSourceFunction.GenerateSourceFunction($"void {Name}_free({CompiledName} self)", {
            "if (self == NULL)",
            vbTab & "return;",
            "free(self->array);",
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

        'Struct content
        Result.Add("// " & ToString())
        Result.Add("typedef struct " & Name & "{")
        Result.Add(vbTab & TypeListed.CompiledName & "* array;")
        Result.Add(vbTab & "int length;")
        Result.Add(vbTab & "int stackReferences;")
        Result.Add(vbTab & "bool marked;")
        Result.Add(vbTab & $"{CompiledName} next;")
        Result.Add("} " & Name & ";")

        'Return result
        Return Result

    End Function

    '==============================
    '======== CONSTRUCTORS ========
    '==============================
    Protected Overrides ReadOnly Property CompiledConstructor(UncompiledConstructor As IUnCompiledProcedure) As CConstructor
        Get
            Return Nothing
        End Get
    End Property

    '==============================
    '======== SET VARIABLE ========
    '==============================
    'Variable = NewValue
    Public Overrides Function SetVariable(Variable As String, NewValue As String) As String
        Return $"{Name}_set(&{Variable}, {NewValue});"
    End Function

    '==========================================
    '======== ALL COMPILED ARRAY TYPES ========
    '==========================================
    Private Shared AllCompiledArrayTypes As New List(Of ArrayType)

    '===========================
    '======== FROM TYPE ========
    '===========================
    Public Shared ReadOnly Property FromType(Type As Type) As ArrayType
        Get

            'If type already compiled
            For Each CompiledArrayType As ArrayType In AllCompiledArrayTypes
                If CompiledArrayType.TypeListed = Type Then
                    Return CompiledArrayType
                End If
            Next

            'Create type
            Dim Result As New ArrayType(Type)
            AllCompiledArrayTypes.Add(Result)
            Return Result

        End Get
    End Property

End Class
