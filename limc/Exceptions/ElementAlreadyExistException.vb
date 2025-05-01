Public Class ElementAlreadyExistException
    Inherits LocalizedException
    Public Sub New(Msg As String, Location As Location)
        MyBase.New("Element already exist", Msg, Location)
    End Sub
    Public Sub New(Location As Location, ElementName As String, Optional ElementType As String = "element")
        MyBase.New($"This {ElementType} already exist", $"The {ElementType} ""{ElementName}"" is already defined.", Location)
    End Sub

    Public Const ELEMENT_PROPERTIE As String = "propertie"
    Public Const ELEMENT_GETTER As String = "getter"
    Public Const ELEMENT_SETTER As String = "setter"
    Public Const ELEMENT_FUNCTION As String = "function"
    Public Const ELEMENT_METHOD As String = "method"
    Public Const ELEMENT_CLASS As String = "class"
    Public Const ELEMENT_STRUCT As String = "struct"
    Public Const ELEMENT_ENUM As String = "enum"
End Class
