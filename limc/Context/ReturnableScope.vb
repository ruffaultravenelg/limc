Public Class ReturnableScope
    Inherits Scope
    Public Property ReturnType As TypeSystem.Type

    Public Sub New(Parent As Context)
        MyBase.New(Parent)
    End Sub

End Class
