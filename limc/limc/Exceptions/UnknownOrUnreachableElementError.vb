Public Class UnknownOrUnreachableElementError
    Inherits LocatedError
    Public Sub New(ElementName As String, Location As Location)
        MyBase.New($"""{ElementName}"" is unreachable", $"The ""{ElementName}"" element cannot be found. Check the spelling or visibility of the element.", Location)
    End Sub
End Class
