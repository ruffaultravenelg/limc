Public Class InternalError
    Inherits BasicException
    Public Sub New(DebbugMessage As String)
        MyBase.New("Internal compiler failure", "This error probably does not originate from your code but from a compiler failure.")
    End Sub

End Class
