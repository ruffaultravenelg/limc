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

        'Functions
        Public ReadOnly Functions As FunctionContainer

        'Types
        Public ReadOnly Types As TypeContainer

        'Constructor
        Private Sub New(Filename As String)

            'Set path
            Me.FullFilePath = IO.Path.GetFullPath(Filename)

            'Parse text
            Dim Lines As IEnumerable(Of SourceLine) = SourceLine.Load(Me)

            'Parse tokens
            Dim Content As FileAST = AST.GenerateAST(Lines)

            'Get functions
            Me.Functions = New FunctionContainer(Content.GetConstructs(Of Source.Function))

            'Get types
            Me.Types = New TypeContainer(Content.GetConstructs(Of TypeConstruct))

        End Sub

        'Load source file
        Public Shared Function Load(Filename As String) As SourceFile
            Return New SourceFile(Filename)
        End Function

    End Class

End Namespace