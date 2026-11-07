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
        Public ReadOnly Property Classes As IReadOnlyList(Of ClassConstruct)

        Public Sub New(
            AbsoluteFilePath As String,
            ImportNodes As IEnumerable(Of ImportFileConstruct),
            Functions As IEnumerable(Of FunctionConstruct),
            Classes As IEnumerable(Of ClassConstruct)
        )
            MyBase.New(New Location(AbsoluteFilePath))
            Me.ImportNodes = ImportNodes.ToList()
            Me.Functions = Functions.ToList()
            Me.Classes = Classes.ToList()
        End Sub

        Public Overloads Overrides Function Accept(Of D, R)(Visitor As IConstructVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitFileNode(Me, Data)
        End Function

    End Class
End Namespace