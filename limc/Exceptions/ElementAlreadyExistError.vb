Public Class ElementAlreadyExistError
    Inherits LocatedError

    Public Sub New(Name As String, Location As Location)
        MyBase.New("Element already exists", $"An element named ""{Name}"" has already been defined in the same scope.", Location)
    End Sub

End Class
