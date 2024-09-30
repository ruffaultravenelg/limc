Public Class CHeapClassConstructor
    Inherits CConstructor

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Type As ClassType, Node As MethodConstructNode, Optional DefaultMethod As Boolean = False)
        MyBase.New(Type, Node, DefaultMethod)
    End Sub

    '=============================
    '======== CREATE SELF ========
    '=============================
    Protected Overrides Function CreateSelf() As IEnumerable(Of String)
        Return {
            "// Allocate memory",
            $"{ParentType.CompiledName} cself = malloc(sizeof({ParentType.Name}));",
            "if (cself == NULL) lim_panic(""Not enough memory"");",
            "",
            "// Set default values",
            "void* self = cself;",
            "cself->stackReferences = 0;",
            "cself->marked = false;",
            "cself->next = NULL;",
            "",
            "// Add it to garbage collector collection",
            "if (" & ParentType.Name & "_head == NULL){",
            $"{vbTab} {ParentType.Name}_head = cself;",
            "} else {",
            $"{vbTab}{ParentType.CompiledName} current = {ParentType.Name}_head;",
            vbTab & "while (current->next != NULL){",
            vbTab & vbTab & "current = current->next;",
            vbTab & "}",
            vbTab & "current->next = cself;",
            "}"
        }
    End Function

    '==============================
    '======== RETURN VALUE ========
    '==============================
    Protected Overrides Function ReturnValue() As String
        Return "cself"
    End Function

End Class
