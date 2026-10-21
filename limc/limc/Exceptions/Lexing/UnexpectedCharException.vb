Public Class UnexpectedCharException
    Inherits LocatedException

    Friend Sub New(Character As Char, Location As Location)
        MyBase.New("L01", "LEXING ERROR", $"Unxpected char : ""{Character}""", Location)
    End Sub

End Class
