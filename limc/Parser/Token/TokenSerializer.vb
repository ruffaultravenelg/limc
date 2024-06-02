Imports System.IO

Public Module TokenSerializer

    Public Sub ParseFileAndSaveTokens(input As String, output As String)

        'Get lines
        Dim Lines As List(Of LimSourceLine) = LineParser.Parse(input)

        'Convert to tokens
        Dim Tokens As List(Of Token) = TokenParser.Parse(Nothing, Lines)

        'Create file
        Using stream As New FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None)
            Using writer As New BinaryWriter(stream)

                'Write number of tokens
                writer.Write(Tokens.Count)

                'Write each token
                For Each tok As Token In Tokens
                    tok.Serialize(writer)
                Next

            End Using
        End Using

    End Sub

    Public Function LoadTokensFromFile(File As LimSource) As List(Of Token)

        'Create result
        Dim tokens As New List(Of Token)

        'Create location
        Dim Location As Location = New FileLocation(File)

        'Read each token
        Using stream As New FileStream(File.Filepath, FileMode.Open, FileAccess.Read, FileShare.None)
            Using reader As New BinaryReader(stream)

                'Read number of tokens
                Dim tokenCount As Integer = reader.ReadInt32()

                'Read each token
                For i As Integer = 0 To tokenCount - 1
                    tokens.Add(Token.Deserialize(reader, Location))
                Next

            End Using
        End Using

        'Return result
        Return tokens

    End Function


End Module
