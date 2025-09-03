Public Class TypeMismatchError
    Inherits LocatedError

    Public Sub New(WantedType As TypeSystem.Type, GivenType As TypeSystem.Type, Location As Location)
        MyBase.New("Type mismatch", $"Expected type ""{WantedType}"", but got type ""{GivenType}"" instead.", Location)
    End Sub

End Class
