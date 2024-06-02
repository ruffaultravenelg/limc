Public Class FileLocation
    Inherits Location

    Public Sub New(File As LimSource)
        MyBase.New(File, -1, -1, -1, -1)
    End Sub

End Class
