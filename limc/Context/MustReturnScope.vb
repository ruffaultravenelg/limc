Namespace Context
    Public Class MustReturnScope
        Inherits Scope

        Public ReadOnly Property ReturnType As TypeSystem.Type

        Public Sub New(Parent As Context, Location As Location, ReturnType As TypeSystem.Type)
            MyBase.New(Parent, Location)
            Me.ReturnType = ReturnType
        End Sub

    End Class
End Namespace