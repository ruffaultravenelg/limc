Public Class ConstantAlreadyExistError
    Inherits LocatedError

    Public Sub New(ConstantName As String, Location As Location)
        MyBase.New("This constant already exists.", $"A constant named ""{ConstantName}"" is already defined in this context.", Location)
    End Sub

End Class
