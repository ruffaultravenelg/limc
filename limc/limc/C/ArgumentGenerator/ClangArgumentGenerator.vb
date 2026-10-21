Imports limc.HIR

Namespace C
    Friend Class ClangArgumentGenerator
        Implements IArgumentGenerator

        Public Function GenerateStartInfo(Program As ProgramNode, CSourcePath As String, OutputFilename As String) As ProcessStartInfo Implements IArgumentGenerator.GenerateStartInfo
            Dim Arguments As New List(Of String)

            ' Main generated file
            Arguments.Add($"-o ""{OutputFilename}""")

            ' Header
            Arguments.Add($"-I""{RuntimeFiles.GetHeaderDirectory()}""")

            ' C files
            Arguments.Add($"""{CSourcePath}""")
            For Each filepath In RuntimeFiles.GetSrcFiles()
                Arguments.Add($"""{filepath}""")
            Next

            ' Lib
            Arguments.Add("-lm") 'Add math lib

            ' Create start info
            Return New ProcessStartInfo() With {
                .FileName = "gcc",
                .Arguments = String.Join(" ", Arguments),
                .UseShellExecute = False,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .CreateNoWindow = True
            }
        End Function

    End Class

End Namespace