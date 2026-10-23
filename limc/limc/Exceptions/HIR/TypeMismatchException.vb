Public Class TypeMismatchException
    Inherits LocatedException

    Friend Sub New(Code As String, GivenType As Symbol.TypeSymbol, ExcpectedType As Symbol.TypeSymbol, Location As Location)
        MyBase.New(Code, "Type mismatch", $"The given type is ""{GivenType}"" whereas the expected type was ""{ExcpectedType}""", Location)
    End Sub
    Friend Sub New(Code As String, GivenType As Symbol.TypeSymbol, FollowingMessage As String, Location As Location)
        MyBase.New(Code, "Type mismatch", $"The given type is ""{GivenType}"" {FollowingMessage}", Location)
    End Sub

End Class
