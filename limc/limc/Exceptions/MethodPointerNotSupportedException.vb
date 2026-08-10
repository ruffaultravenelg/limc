Public Class MethodPointerNotSupportedException
    Inherits LocatedError

    Public Sub New(Location As Location)
        MyBase.New("Method pointers are not supported", "limc does not support method pointers for now, as they can lead to complex pointer types that are hard to manage for the garbage collector", Location)
    End Sub

End Class
