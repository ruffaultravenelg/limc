Namespace AST
    Public MustInherit Class ImportNode
        Inherits Node

        Private AbsoluteFilepath As String
        Private FileCache As Context.SourceFile = Nothing

        Public ReadOnly Property AssociatedFile As Context.SourceFile
            Get
                If FileCache Is Nothing Then
                    FileCache = Context.SourceFile.FromFile(AbsoluteFilepath)
                End If
                Return FileCache
            End Get
        End Property

        Protected Sub SetAbsoluteFilepath(RealPath As String, InitialValue As String)
            If (Not IO.File.Exists(RealPath)) Then
                Throw New IncludedFileDoesntExistError(Location, InitialValue, RealPath)
            Else
                AbsoluteFilepath = RealPath
            End If
        End Sub

        Protected Sub New(Location As Location)
            MyBase.New(Location)
        End Sub

    End Class
End Namespace