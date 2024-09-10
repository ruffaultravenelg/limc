Public Class CPrimitiveClassConstructor
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
            $"{ParentType.CompiledName} cself;",
            $"void* self = &cself;"
        }
    End Function

    '==============================
    '======== RETURN VALUE ========
    '==============================
    Protected Overrides Function ReturnValue() As String
        Return "cself"
    End Function

End Class
