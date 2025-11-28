Public Class ResourceUsedTooQuicklyError
    Inherits LocatedError

    Public Sub New(Location As Location)
        MyBase.New("This resource was accessed too quickly.", "This resource was used before it could be fully defined. Try to specify the types explicitly, or set explicit values.", Location)
    End Sub

End Class
