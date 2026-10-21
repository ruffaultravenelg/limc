Public Class InternalException
    Inherits Exception

    Protected ReadOnly Title As String

    Friend Sub New(Title As String, Message As String)
        MyBase.New(Message)
        Me.Title = Title
    End Sub

    Public Sub New(Message As String)
        Me.New("Internal Exception", Message)
    End Sub

End Class
