Public Module Compiler

    Private _MainFile As LimSource
    Public ReadOnly Property MainFile As LimSource
        Get
            Return _MainFile
        End Get
    End Property

    Public Sub Compile()

        'Parse main file
        _MainFile = New LimSource(Program.InputFile)

    End Sub

End Module
