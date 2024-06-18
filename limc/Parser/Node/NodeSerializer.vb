Imports System.IO
Imports System.IO.Compression
Imports System.Text.Json

Public Module NodeSerializer

    '
    ' Méthode pour sérialiser et compresser un AST dans un fichier
    '
    Public Sub SaveSourceToFile(File As LimSource, filePath As String)
        Using fileStream As New FileStream(filePath, FileMode.Create)
            Using gzipStream As New GZipStream(fileStream, CompressionMode.Compress)
                Dim options As New JsonSerializerOptions With {.WriteIndented = False, .IncludeFields = True}

                JsonSerializer.Serialize(gzipStream, File.Exceptions, options) 'Exceptions

            End Using
        End Using
    End Sub

    '
    ' Méthode pour désérialiser et décompresser un AST depuis un fichier
    '
    Public Sub LoadSourceFile(File As LimSource, filePath As String)
        Using fileStream As New FileStream(filePath, FileMode.Open)
            Using gzipStream As New GZipStream(fileStream, CompressionMode.Decompress)
                Dim options As New JsonSerializerOptions With {.IncludeFields = True}

                File.Exceptions.AddRange(JsonSerializer.Deserialize(Of List(Of Node))(gzipStream, options)) 'Exceptions

            End Using
        End Using
    End Sub

    '
    ' Méthode pour sérialiser un AST dans un fichier
    '
    Public Sub SaveSourceToJsonFile(File As LimSource, filePath As String)
        Using fileStream As New FileStream(filePath, FileMode.Create)
            Dim options As New JsonSerializerOptions With {.WriteIndented = True, .IncludeFields = True}

            JsonSerializer.Serialize(fileStream, File.Exceptions, options) 'Exceptions

        End Using
    End Sub

    '
    ' Méthode pour désérialiser un AST depuis un fichier
    '
    Public Sub LoadSourceJsonFile(File As LimSource, filePath As String)
        Using fileStream As New FileStream(filePath, FileMode.Open)
            Dim options As New JsonSerializerOptions With {.IncludeFields = True}

            File.Exceptions.AddRange(JsonSerializer.Deserialize(Of List(Of Node))(fileStream, options)) 'Exceptions

        End Using
    End Sub


End Module