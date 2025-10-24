Public Class ProcedureDontReturnValueError
    Inherits LocatedError

    Public Sub New(ProcedureLocation As Location, ProcedureName As String)
        MyBase.New("This function doesn't return any value", $"The function '{ProcedureName}' was called by an expression, but it does not return any value. It may be that lazy evaluation has not finished examining the body of the function. Consider adding an explicit return type to the function definition.", ProcedureLocation)
    End Sub

End Class
