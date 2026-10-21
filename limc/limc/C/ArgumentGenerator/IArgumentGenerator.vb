Namespace C
    Friend Interface IArgumentGenerator

        Function GenerateStartInfo(Program As HIR.ProgramNode, CSourcePath As String, OutputFilename As String) As ProcessStartInfo

    End Interface
End Namespace