Namespace AST
    '
    ' Represent the whole program, contains all source files
    '
    Friend Class ProgramNode
        Inherits ConstructNode

        Public ReadOnly Property EntryFile As FileNode

        Public Sub New(EntryFile As FileNode)
            MyBase.New(EntryFile.Location)
            Me.EntryFile = EntryFile
        End Sub

        Public Overloads Overrides Function Accept(Of D, R)(Visitor As IConstructVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitProgramNode(Me, Data)
        End Function

    End Class

End Namespace