Public Class VariableAlreadyExistError
    Inherits LocatedError

    Public Sub New(VariableName As String, Location As Location)
        MyBase.New("This variable already exists.", $"A variable named ""{VariableName}"" is already defined in this context.", Location)
    End Sub

End Class
