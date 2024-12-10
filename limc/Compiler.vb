Friend Class Compiler

    'Inputs
    Private ReadOnly Input As String
    Private ReadOnly Output As String

    'Constructor
    Public Sub New(Input As String, Output As String)
        Me.Input = Input
        Me.Output = Output
    End Sub

    'Compile the input file
    Friend Sub Compile()

        'Parse file
        Dim Source As Lim.SourceFile = Lim.SourceFile.Load(Input)

    End Sub

End Class
