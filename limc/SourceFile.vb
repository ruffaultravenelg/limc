Namespace Lim

    Public Class SourceFile

        'File path
        Public ReadOnly Property FullFilePath As String

        'File name
        Public ReadOnly Property Filename As String
            Get
                Return IO.Path.GetFileName(FullFilePath)
            End Get
        End Property

        'Relative path
        Public ReadOnly Property RelativePath As String
            Get
                Return IO.Path.GetRelativePath(IO.Directory.GetCurrentDirectory(), FullFilePath)
            End Get
        End Property

        'Constructor
        Private Sub New(Filename As String)

            'Set path
            Me.FullFilePath = IO.Path.GetFullPath(Filename)

            'Parse text
            Dim Lines As IEnumerable(Of SourceLine) = SourceLine.Load(Me)

            'Parse tokens
            Dim Content As FileAST = AST.GenerateAST(Lines)

            'Print
            Console.WriteLine(Content.ToString())

        End Sub

        'Load source file
        Public Shared Function Load(Filename As String) As SourceFile
            Return New SourceFile(Filename)
        End Function

    End Class

End Namespace