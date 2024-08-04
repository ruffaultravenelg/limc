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
    Protected Overrides Function CreateSelf() As String
        Return $"{ParentType.CompiledName} self;"
    End Function

End Class
