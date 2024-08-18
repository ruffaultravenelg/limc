Public Module CommandBuilder

    Public ReadOnly Property Command As String
        Get

            'Write output
            Dim Result As String = "gcc -o program"

            'Add source file
            Result &= " """ & Compiler.SourcePath & """"

            'Add each custom include files
            For Each ImportedFile As String In CustomInclude.All
                Result &= " """ & ImportedFile & """"
            Next

            'Return result
            Return Result

        End Get
    End Property


End Module
