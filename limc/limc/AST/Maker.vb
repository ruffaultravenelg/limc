Imports limc.Repository

Namespace AST
    Friend Class Maker

        Public Shared Function GenerateAST(SourceFile As String) As ProgramNode

            Dim FileRepo As New FileRepository()
            Dim FileInfo As New IO.FileInfo(SourceFile)

            If Not FileInfo.Exists Then
                Throw New RenderableException("PM01", "File not found", $"Input lim file not found : ""{SourceFile}"".")
            End If

            ' Generate entry file
            Dim EntryFile As FileNode = FileRepo.LoadFile(FileInfo.Name, FileInfo.DirectoryName)

            ' Generate each imported file recursivly
            ResolveFileImport(EntryFile, FileRepo, New HashSet(Of FileNode))

            Return New ProgramNode(EntryFile)

        End Function

        ' postfix 
        Private Shared Sub ResolveFileImport(File As FileNode, FileRepo As FileRepository, ResolvedFiles As ISet(Of FileNode))

            ' Skip already finished files
            If ResolvedFiles.Contains(File) Then
                Exit Sub
            End If

            ' Add ourself to finished files
            ResolvedFiles.Add(File)

            ' Load each imported files and resolve theres imports recursivly
            ' TODO: use visitor if more than one import method
            Dim CurrentFileFolder As String = New IO.FileInfo(File.Filepath).DirectoryName
            For Each ImportNode In File.ImportNodes
                ImportNode._SetFileNode(FileRepo.LoadFile(ImportNode.Filename, CurrentFileFolder))
                ResolveFileImport(ImportNode.FileNode, FileRepo, ResolvedFiles)
            Next

        End Sub

    End Class
End Namespace