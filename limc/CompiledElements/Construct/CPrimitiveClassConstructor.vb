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
            $"{ParentType.CompiledName} local_self;",
            $"void* self = &local_self;"
        }
    End Function

    '==============================
    '======== RETURN VALUE ========
    '==============================
    Protected Overrides Function ReturnValue() As String
        Return "local_self"
    End Function

End Class
