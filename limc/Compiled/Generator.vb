Namespace C
    Public Module Generator

        'Naming shema
        Public ReadOnly Namer As New CounterNamer()

        'Content of the file
        Private Import As New HashSet(Of String)
        Private Functions As New HashSet(Of C.Function)

        'Append function
        Public Sub AddFunction(Fn As C.Function)
            Functions.Add(Fn)
        End Sub

        'Append import
        Public Sub AddImport(Import As String)
            If Not Generator.Import.Contains(Import) Then
                Generator.Import.Add(Import)
            End If
        End Sub

        'Write
        Public Sub Write(Stream As IO.StreamWriter)

            'Write file header
            Stream.WriteLine("/*")
            Stream.WriteLine("")
            Stream.WriteLine(vbTab & "File compiled by Lim compiler.")
            Stream.WriteLine(vbTab & "Written by Gémino Ruffault--Ravenel the 22/12/2024")
            Stream.WriteLine("")
            Stream.WriteLine(vbTab & "You are the only responsible for this file and his content.")
            Stream.WriteLine("")
            Stream.WriteLine(vbTab & "Compile using :")
            Stream.WriteLine(vbTab & vbTab & "gcc source.c -o prog")
            Stream.WriteLine("")
            Stream.WriteLine("*/")

            'Write imports
            WriteTitle(Stream, "Imports")
            For Each Import As String In Generator.Import
                Stream.WriteLine(Import)
            Next

            'Write functions signatures
            WriteTitle(Stream, "Functions signatures")
            For Each Func As C.Function In Functions
                Func.WriteSignature(Stream)
            Next

            'Write functions bodies
            Stream.WriteLine()
            WriteTitle(Stream, "Functions bodies")
            For Each Func As C.Function In Functions
                Func.WriteBody(Stream)
            Next

        End Sub

        'Write title
        Private Sub WriteTitle(Stream As IO.StreamWriter, Title As String)

            'Generate title
            Dim Main As String = "//// " & Title.ToUpper() & " ////"
            Dim Around As String = StrDup(Main.Length, "/")

            'Write
            Stream.WriteLine()
            Stream.WriteLine(Around)
            Stream.WriteLine(Main)
            Stream.WriteLine(Around)

        End Sub

    End Module
End Namespace
