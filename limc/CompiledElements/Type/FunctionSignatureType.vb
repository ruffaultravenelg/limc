'
' Represent a fun class type
'     fun<str, bool><int>
'
Public Class FunctionSignatureType
    Inherits Type
    Implements BuildableStructure

    '=============================
    '======== SOURCE NAME ========
    '=============================
    Protected Overrides ReadOnly Property SourceName As String
        Get
            Return "fun"
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

        'Create function builder
        CSourceFunction.GenerateSourceFunction($"{Name} {Name}_new_func({FunctionPointerType_C})", {
            $"{Name} wrapper;",
            "wrapper.func = func;",
            "wrapper.obj = NULL;",
            "wrapper.is_method = 0;",
            "return wrapper;"
        })

        'Create method builder
        CSourceFunction.GenerateSourceFunction($"{Name} {Name}_new_method(void* obj, {MethodPointerType_C})", {
            $"{Name} wrapper;",
            "wrapper.method = method;",
            "wrapper.obj = obj;",
            "wrapper.is_method = 1;",
            "return wrapper;"
        })

        'Create default
        CSourceFunction.GenerateSourceFunction($"{Name} {Name}_default()", {
            $"{Name} wrapper;",
            "wrapper.func = NULL;",
            "wrapper.obj = NULL;",
            "wrapper.is_method = 0;",
            "return wrapper;"
        })

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
            "if (self.is_method)",
            $"{vbTab}self.method(self.obj{If(CompiledArgs = "", "", ", ")}{CompiledArgs});",
            "else",
            $"{vbTab}if (self.func == NULL) lim_panic(""{NullPtrExceptionMessage}""); else self.func({CompiledArgs});"
        })
        Else
            CSourceFunction.GenerateSourceFunction($"{ReturnType.CompiledName} {Name}_call({CompiledArgumentsForCall})", {
            "if (self.is_method)",
            $"{vbTab}return self.method(self.obj{If(CompiledArgs = "", "", ", ")}{CompiledArgs});",
            "else",
            $"{vbTab}if (self.func == NULL) lim_panic(""{NullPtrExceptionMessage}""); else return self.func({CompiledArgs});"
        })
        End If

    End Sub

    '===============================
    '======== NEW FUNCTIONS ========
    '===============================
    Public ReadOnly Property NewFuncCompiledName As String = Name & "_new_func"
    Public ReadOnly Property NewMethodCompiledName As String = Name & "_new_method"

    '===============================
    '======== CALL FUNCTION ========
    '===============================
    Public ReadOnly Property CallCompiledName As String = Name & "_call"

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

        'Custom content
        Result.Add("// " & ToString())
        Result.Add("typedef struct " & Name & "{")
        Result.Add(vbTab & "union {")
        Result.Add(vbTab & vbTab & FunctionPointerType_C & ";")
        Result.Add(vbTab & vbTab & MethodPointerType_C & ";")
        Result.Add(vbTab & "};")
        Result.Add(vbTab & "void* obj;")
        Result.Add(vbTab & "int is_method;")
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
    Public Overrides ReadOnly Property DefaultValue As String
        Get
            Return $"{Name}_default()"
        End Get
    End Property

End Class