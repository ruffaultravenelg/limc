Public Class NotMainFunctionError
    Inherits BasicException

    Public Sub New(File As SourceFile)
        MyBase.New("The file does not contain an entry point.", $"The file ""{File.RelativePath}"" does not contain a ""main"" function.")
    End Sub

End Class
