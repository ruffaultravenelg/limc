Public Class UnanticipatedException
    Inherits CompilerException

    Public Sub New()
        MyBase.New("Unanticipated exception", "It seems that something has slipped into a place it shouldn't be. This is a compiler error. Please report it to the developer.")
    End Sub

End Class
