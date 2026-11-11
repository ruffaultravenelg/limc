Public Class ExpressionIsntLvalException
    Inherits LocatedException

    Friend Sub New(Code As String, Location As Location)
        MyBase.New(Code, "Expected a lvalue", $"The given expression cannot be resolved as a lvalue", Location)
    End Sub

End Class
