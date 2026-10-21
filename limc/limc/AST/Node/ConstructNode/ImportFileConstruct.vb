Namespace AST
    Friend Class ImportFileConstruct
        Inherits ConstructNode

        Public ReadOnly Property Filename As String
        Public ReadOnly Property FileNode As FileNode

        Public Sub New(Filename As String, Location As Location)
            MyBase.New(Location)
            Me.Filename = Filename
            Me.FileNode = Nothing ' Will be set by AST.Maker.GenerateAST
        End Sub

        Friend Sub _SetFileNode(Node As FileNode)
            _FileNode = Node
        End Sub

        Public Overloads Overrides Function Accept(Of D, R)(Visitor As IConstructVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitImportFileConstruct(Me, Data)
        End Function
    End Class

End Namespace