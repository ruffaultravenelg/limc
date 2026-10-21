Namespace AST
    '
    ' Represent a Source file, ex: main.lim
    '
    Friend Class FileNode
        Inherits ConstructNode

        Public ReadOnly Property Filepath As String
            Get
                Return Me.Location.Filename
            End Get
        End Property

        Public ReadOnly Property ImportNodes As IReadOnlyList(Of ImportFileConstruct)
        Public ReadOnly Property Functions As IReadOnlyList(Of FunctionConstruct)

        Public Sub New(
            AbsoluteFilePath As String,
            ImportNodes As IEnumerable(Of ImportFileConstruct),
            Functions As IEnumerable(Of FunctionConstruct)
        )
            MyBase.New(New Location(AbsoluteFilePath))
            Me.ImportNodes = ImportNodes
            Me.Functions = Functions
        End Sub

        Public Overloads Overrides Function Accept(Of D, R)(Visitor As IConstructVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitFileNode(Me, Data)
        End Function

    End Class
End Namespace