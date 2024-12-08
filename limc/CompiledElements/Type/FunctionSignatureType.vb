'
' Represent a fun class type
'     fun<str, bool><int>
'
Public Class FunctionSignatureType
    Inherits Type
    Implements IBuildableStructure
    Implements IGarbageCollectedType

    '=============================
    '======== SOURCE NAME ========
    '=============================
    Protected Overrides ReadOnly Property SourceName As String
        Get
            Return "fun"
        End Get
    End Property

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

    '=================================
    '======== ARGUMENTS TYPES ========
    '=================================
    Public ReadOnly Property ArgumentsTypes As IEnumerable(Of Type)

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Public ReadOnly Property ReturnType As Type

    '=================================
    '======== POINTER C TYPES ========
    '=================================
    Private FunctionPointerType_C As String
    Private MethodPointerType_C As String

    '============================================
    '======== NULL PTR EXCEPTION MESSAGE ========
    '============================================
    Private Const NullPtrExceptionMessage As String = "A piece of code tried to call a null function pointer."

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(ArgumentsTypes As IEnumerable(Of Type), ReturnType As Type)
        MyBase.New(LimSource.STD)

        'Set values
        Me.ArgumentsTypes = ArgumentsTypes
        Me.ReturnType = ReturnType

        'Notify FileBuilder of this new structure
        FileBuilder.NotifyNewStructure(Me)

        'Create return type
        Dim CompiledReturnType As String = If(ReturnType Is Nothing, "void", ReturnType.CompiledName)

        'Compile arguments
        Dim CompiledArgumentsType As String = ""
        For Each ArgumentType As Type In ArgumentsTypes
            CompiledArgumentsType &= ", " & ArgumentType.CompiledName
        Next
        If ArgumentsTypes.Count > 0 Then
            CompiledArgumentsType = CompiledArgumentsType.Substring(2)
        End If

        'Create function pointer
        FunctionPointerType_C = $"{CompiledReturnType} (*func)({CompiledArgumentsType})"

        'Create method pointer
        Dim MethodSelfArgument As String = If(ArgumentsTypes.Count > 0, "void*, ", "void*")
        MethodPointerType_C = $"{CompiledReturnType} (*method)({MethodSelfArgument}{CompiledArgumentsType})"

        'Compile arguments for call function
        Dim CompiledArgumentsForCall As String = CompiledName & " self"
        Dim CompiledArgs As String = ""
        For i As Integer = 0 To ArgumentsTypes.Count - 1
            CompiledArgumentsForCall &= ", " & ArgumentsTypes(i).CompiledName & " arg" & i.ToString()
            CompiledArgs &= ", arg" & i.ToString()
        Next
        If ArgumentsTypes.Count > 0 Then
            CompiledArgs = CompiledArgs.Substring(2)
        End If

        'Create caller
        If ReturnType Is Nothing Then
            CSourceFunction.GenerateSourceFunction($"void {Name}_call({CompiledArgumentsForCall})", {
            "if (self->is_method)",
            $"{vbTab}self->method(self->obj{If(CompiledArgs = "", "", ", ")}{CompiledArgs});",
            "else",
            $"{vbTab}if (self->func == NULL) lim_panic(""{NullPtrExceptionMessage}""); else self->func({CompiledArgs});"
        })
        Else
            CSourceFunction.GenerateSourceFunction($"{ReturnType.CompiledName} {Name}_call({CompiledArgumentsForCall})", {
            "if (self.is_method)",
            $"{vbTab}return self.method(self.obj{If(CompiledArgs = "", "", ", ")}{CompiledArgs});",
            "else",
            $"{vbTab}if (self.func == NULL) lim_panic(""{NullPtrExceptionMessage}""); else return self.func({CompiledArgs});"
        })
        End If

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
        CSourceFunction.GenerateSourceFunction($"void {Name}_mark({CompiledName} self)", {
            "if (self == NULL || self->marked)",
            vbTab & "return;",
            "",
            "self->marked = true;",
            "if (self->obj == NULL) return;",
            "if (self->mark) self->mark(self->obj);"
        })

        'Create free function
        CSourceFunction.GenerateSourceFunction($"void {Name}_free({CompiledName} self)", {
            "if (self == NULL)",
            vbTab & "return;",
            "if (self->obj == NULL) return;",
            "if (self->free) self->free(self->obj);",
            "free(self);"
        })

    End Sub

    '==========================================
    '======== BUILD FUNCTION REFERENCE ========
    '==========================================
    Private FunctionReferenceBuilder_CompiledName As String = Nothing

    ' Create a instance if this function reference that point to a function
    Public Function BuildFunctionReference(FunctionCompiledName As String) As String

        'If function is not created
        If FunctionReferenceBuilder_CompiledName = Nothing Then
            FunctionReferenceBuilder_CompiledName = GenerateFunctionReferenceBuilder()
        End If

        'Compile call
        Return FunctionReferenceBuilder_CompiledName & "(" & FunctionCompiledName & ")"

    End Function

    'Build the default function reference builder func
    Private Function GenerateFunctionReferenceBuilder() As String

        'Generate function name
        Dim FnName As String = $"{Name}_new_fromFunction"

        'Append function
        CSourceFunction.GenerateSourceFunction(
            $"{CompiledName} {FnName}({FunctionPointerType_C})",
            {
                $"// Instanciate",
                $"{CompiledName} self = malloc(sizeof({Name}));",
                $"if (self == NULL) lim_panic(""Not enought memory"");",
                $"",
                $"// Init values",
                $"self->func = func;",
                $"self->is_method = 0;",
                $"self->obj = NULL;",
                $"self->mark = NULL;",
                $"self->free = NULL;",
                $"self->stackReferences = 0;",
                $"self->marked = false;",
                $"self->next = NULL;",
                $"",
                $"// Add it to garbage collector collection",
                $"if ({Name}_head == NULL){{",
                $"{vbTab}{Name}_head = self;",
                $"}} else {{",
                $"{vbTab}{CompiledName} current = {Name}_head;",
                $"{vbTab}while (current->next != NULL){{",
                $"{vbTab}{vbTab}current = current->next;",
                $"{vbTab}}}",
                $"{vbTab} current->next = self;",
                $"}}",
                $"",
                $"// Return instance",
                $"return self;"
            }
        )

        'Return function name
        Return FnName

    End Function


    '========================================
    '======== BUILD METHOD REFERENCE ========
    '========================================
    Private MethodReferenceBuilder_CompiledName As String = Nothing

    ' Create a instance if this emethod reference that point to a function
    Public Function BuildMethodReference(MethodCompiledName As String, ObjectType As ClassType) As String

        '

        'If function is not created
        If MethodReferenceBuilder_CompiledName = Nothing Then
            MethodReferenceBuilder_CompiledName = GenerateMethodReferenceBuilder()
        End If

        'Compile call
        Return MethodReferenceBuilder_CompiledName & "(" & MethodCompiledName & ")"

    End Function

    'Build the default method reference builder func
    Private Function GenerateMethodReferenceBuilder() As String

        'Generate function name
        Dim FnName As String = $"{Name}_new_fromFunction"

        'Append function
        CSourceFunction.GenerateSourceFunction(
            $"{CompiledName} {FnName}({FunctionPointerType_C})",
            {
                $"// Instanciate",
                $"{CompiledName} self = lim_malloc(sizeof({Name}));",
                $"",
                $"// Init values",
                $"self->func = func;",
                $"self->is_method = 0;",
                $"self->obj = NULL;",
                $"self->mark = NULL;",
                $"self->free = NULL;",
                $"self->stackReferences = 0;",
                $"self->marked = false;",
                $"self->next = NULL;",
                $"",
                $"// Add it to garbage collector collection",
                $"if ({Name}_head == NULL){{",
                $"{vbTab}{Name}_head = self;",
                $"}} else {{",
                $"{vbTab}{CompiledName} current = {Name}_head;",
                $"{vbTab}while (current->next != NULL){{",
                $"{vbTab}{vbTab}current = current->next;",
                $"{vbTab}}}",
                $"{vbTab} current->next = self;",
                $"}}",
                $"",
                $"// Return instance",
                $"return self;"
            }
        )

        'Return function name
        Return FnName

    End Function

    '===============================
    '======== NEW FUNCTIONS ========
    '===============================
    Public ReadOnly Property NewMethodCompiledName As String = Name & "_new_method"

    '===============================
    '======== CALL FUNCTION ========
    '===============================
    Public ReadOnly Property CallCompiledName As String = Name & "_call"

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

        'Custom content
        Result.Add("// " & ToString())
        Result.Add("typedef struct " & Name & "{")
        Result.Add(vbTab & "union {")
        Result.Add(vbTab & vbTab & FunctionPointerType_C & ";")
        Result.Add(vbTab & vbTab & MethodPointerType_C & ";")
        Result.Add(vbTab & "};")
        Result.Add(vbTab & "int is_method;")
        Result.Add(vbTab & "void* obj;")
        Result.Add(vbTab & "void (*mark)(void*);")
        Result.Add(vbTab & "void (*free)(void*);")

        Result.Add(vbTab & "unsigned long stackReferences;")
        Result.Add(vbTab & "bool marked;")
        Result.Add(vbTab & $"{CompiledName} next;")
        Result.Add("} " & Name & ";")

        'Return result
        Return Result

    End Function

    '===========================
    '======== TO STRING ========
    '===========================
    Public Overrides Function ToString() As String
        Return $"fun{Type.StringifyListOfType(ArgumentsTypes)}{If(ReturnType Is Nothing, "<>", Type.StringifyListOfType({ReturnType}))}"
    End Function

    '============================
    '======== LOOKS LIKE ========
    '============================
    Public Shadows Function LooksLike(ArgumentsTypes As IEnumerable(Of Type), ReturnType As Type) As Boolean

        'Not the same argument count
        If Not ArgumentsTypes.Count = Me.ArgumentsTypes.Count Then
            Return False
        End If

        'Not the same return type
        If Not Me.ReturnType = ReturnType Then
            Return False
        End If

        'Not the same arguments
        For i As Integer = 0 To ArgumentsTypes.Count - 1
            If Not Me.ArgumentsTypes(i) = ArgumentsTypes(i) Then
                Return False
            End If
        Next

        'Corporate says there are the same
        Return True

    End Function

    '=========================
    '======== DEFAULT ========
    '=========================
    Public Overrides ReadOnly Property DefaultValue As String = "NULL"

    '==============================
    '======== SET VARIABLE ========
    '==============================
    'Variable = NewValue
    Public Overrides Function SetVariable(Variable As String, NewValue As String) As String
        Return $"{Name}_set(&{Variable}, {NewValue});"
    End Function

End Class