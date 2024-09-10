Public Class CArrayConstructor
    Inherits CConstructor

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(ListedType As ArrayType)
        MyBase.New(ListedType, Nothing)

        'Add int variable
        Scope.Variables.Add(New Variable("length", Type.int))

    End Sub

    '=============================
    '======== CREATE SELF ========
    '=============================
    Protected Overrides Function CreateSelf() As IEnumerable(Of String)
        Return {
            "// Allocate memory",
            $"{ParentType.CompiledName} self = malloc(sizeof({ParentType.Name}));",
            "if (self == NULL) lim_panic(""Not enough memory"");",
            "",
            "// Set default values",
            $"self->array = malloc(sizeof({DirectCast(ParentType, ArrayType).Scope.GenericTypes(0).Type.CompiledName}) * length);",
            "if (self->array == NULL) lim_panic(""Not enough memory"");",
            "self->stackReferences = 0;",
            "self->marked = false;",
            "self->next = NULL;",
            "self->length = length;",
            "",
            "// Add it to garbage collector collection",
            "if (" & ParentType.Name & "_head == NULL){",
            $"{vbTab} {ParentType.Name}_head = self;",
            "} else {",
            $"{vbTab}{ParentType.CompiledName} current = {ParentType.Name}_head;",
            vbTab & "while (current->next != NULL){",
            vbTab & vbTab & "current = current->next;",
            vbTab & "}",
            "}"
        }
    End Function

    '==============================
    '======== RETURN VALUE ========
    '==============================
    Protected Overrides Function ReturnValue() As String
        Return "self"
    End Function

    '=================================
    '======== BUILD PROTOTYPE ========
    '=================================
    Protected Overrides Function BuildPrototype() As String
        Return $"{ParentType.CompiledName} {CompiledName}(int length)"
    End Function

End Class
