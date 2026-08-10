Public Class NotTheRightElementException
    Inherits Exception

    Public Sub New()
        MyBase.New("This element is not what this function is looking for.")
    End Sub
End Class
