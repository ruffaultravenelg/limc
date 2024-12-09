Friend Class Compiler

    Private ReadOnly input As String
    Private ReadOnly output As String

    Public Sub New(input As String, output As String)
        Me.input = input
        Me.output = output
    End Sub

    Friend Sub Compile()
        Console.WriteLine(IO.File.ReadAllText(input))
    End Sub
End Class
