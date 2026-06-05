Public Class InternalError
    Inherits BasicException
    Public Sub New(Optional DebugMessage As String = "")
        MyBase.New("Internal compiler failure", If(String.IsNullOrEmpty(DebugMessage), "This error probably does not originate from your code but from a compiler failure.", DebugMessage))
    End Sub

End Class
