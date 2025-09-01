Public Class EndOfFileError
    Inherits LocatedError

    Public Sub New(Location As Location)
        MyBase.New("The file ended unexpectedly.", "The entire code could not be analyzed; the end was not expected so soon.", Location)
    End Sub

End Class
