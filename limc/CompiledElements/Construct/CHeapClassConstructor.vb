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
            $"{ParentType.CompiledName} self = lim_malloc(sizeof({ParentType.Name}));",
            "",
            "// Set default values",
            "self->stackReferences = 0;",
            "self->marked = false;",
            "self->next = NULL;",
            "",
            "// Add it to garbage collector collection",
            "if (" & ParentType.Name & "_head == NULL){",
            $"{vbTab} {ParentType.Name}_head = self;",
            "} else {",
            $"{vbTab}{ParentType.CompiledName} current = {ParentType.Name}_head;",
            vbTab & "while (current->next != NULL){",
            vbTab & vbTab & "current = current->next;",
            vbTab & "}",
            vbTab & "current->next = self;",
            "}"
        }
    End Function

    '==============================
    '======== RETURN VALUE ========
    '==============================
    Protected Overrides Function ReturnValue() As String
        Return "self"
    End Function

End Class
