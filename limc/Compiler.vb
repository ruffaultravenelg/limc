Friend Module Compiler

    'Compile a file to a output source file
    Friend Sub Compile(Input As String, Output As String)

        'Add general imports
        C.Generator.AddImport("#include <stdio.h>")
        C.Generator.AddImport("#include <stdlib.h>")
        C.Generator.AddImport("#include <string.h>")
        C.Generator.AddImport("#include <stdbool.h>")

        'Parse file
        Dim Source As Lim.SourceFile = Lim.SourceFile.Load(Input)

        'Get "main" function
        Dim MainFunction As Lim.Function = Source.Functions.GetCorrespondance("main", {}, {})

        'Write file to output
        Dim Writer As New IO.StreamWriter(Output)
        C.Generator.Write(Writer)
        Writer.Close()

    End Sub

End Module
