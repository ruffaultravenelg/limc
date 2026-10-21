Imports limc.AST

Namespace Repository
    Public Class FileRepository

        Private ReadOnly Files As New Dictionary(Of String, FileNode)

        Public Function LoadFile(Filename As String, FromDirectory As String)

            ' Get absolute filepath
            Dim FullFilename As String = IO.Path.GetFullPath(Filename, FromDirectory)

            ' Check if it exist in cache
            If Files.ContainsKey(FullFilename) Then
                Return Files(FullFilename)
            Else
                Dim NewFile = ReadAndParseFile(FullFilename)
                Files(FullFilename) = NewFile
                Return NewFile
            End If

        End Function

        ' Read / Tokenize / Parse
        Private Function ReadAndParseFile(Filepath As String) As FileNode

            ' Check file exists
            If Not IO.File.Exists(Filepath) Then
                Throw New FileDoesntExistExcepetion()
            End If

            ' Get tokens
            Dim Tokens As IEnumerable(Of Lexer.Token)
            Using Reader As New IO.StreamReader(Filepath)
                Tokens = Lexer.Tokenizer.Parse(Reader, Filepath)
            End Using

            ' Parse AST
            Return FileParser.Parse(Tokens, Filepath)

        End Function

        Public Class FileDoesntExistExcepetion
            Inherits Exception
        End Class

    End Class
End Namespace