Imports System.Runtime.InteropServices

Public Module CAPI

    ' C developer API
    ' Miscellaneous helper functions

    ' Do a line should be kept by it's platform directive name
    Public Function DoKeepLineFromPlatformName(PlatformName As String, Location As Location)

        Select Case PlatformName
            Case "win"
                Return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)

            Case "unix"
                Return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) OrElse RuntimeInformation.IsOSPlatform(OSPlatform.OSX) OrElse RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)

            Case Else
                Throw New SyntaxError($"The ""{PlatformName}"" platform is unknown. Uses ""win"" or ""unix"".", Location)

        End Select

    End Function

    ' Include a $include
    Public Sub HandleInclude(Header As String, Location As Location)

        ' Just a lib
        If Header.StartsWith("<") Then
            CodeGen.RegisterInclude(Header)
            Exit Sub
        End If

        ' A file -> get full path
        Dim FullFilePath As String = IO.Path.GetFullPath(Header, Location.File.Filepath)
        If Not IO.File.Exists(FullFilePath) Then
            Throw New IncludedFileDoesntExistError(Location, Header, FullFilePath)
        End If
        CodeGen.RegisterInclude("""" & FullFilePath & """")

        ' If ends by ".h", try to find ".c"
        Dim ImplementationFile As String = IO.Path.ChangeExtension(FullFilePath, "c")
        If IO.File.Exists(ImplementationFile) Then
            Compiler.CFilesToInclude.Add(ImplementationFile)
        End If

    End Sub

End Module