Public Class ReturnableScope
    Inherits Scope
    Public Property ReturnType As TypeSystem.Type

    Public Sub New(Parent As Context, Location As Location)
        MyBase.New(Parent, Location)
    End Sub

End Class
