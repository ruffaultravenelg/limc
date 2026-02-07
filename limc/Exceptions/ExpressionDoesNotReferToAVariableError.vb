Public Class ExpressionDoesNotReferToAVariableError
    Inherits SyntaxError

    Public Sub New(Location As Location)
        MyBase.New("This expression doesn't point to a variable or a rack index.", Location)
    End Sub

End Class
