Public Class IncludedFileDoesntExistError
    Inherits LocatedError

    Public Sub New(Location As Location, InitialPath As String, RealPath As String)
        MyBase.New("The specified file is not accessible.", $"The path ""{InitialPath}"" result in the following path :{Environment.NewLine}{vbTab}{RealPath}{Environment.NewLine}This file does not exist.", Location)
    End Sub

End Class
