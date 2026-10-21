Public Class UnreachableElementException
    Inherits LocatedException

    Friend Sub New(Code As String, ElementName As String, Location As Location)
        MyBase.New(Code, "Unreachable element", $"""{ElementName}"" is unreachable, check it's visibility or imports.", Location)
    End Sub

End Class
