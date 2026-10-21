Namespace AST
    Friend MustInherit Class Node
        Public ReadOnly Property Location As Location

        Public Sub New(Location As Location)
            Me.Location = Location
        End Sub

        Public MustOverride Function Accept(Of D, R)(Visitor As IASTVisitor(Of D, R), Data As D) As R

        Public Overrides Function ToString() As String
            Using tw As New OutputStream.StringOutputStream()
                Printer.ASTPrinter.PrintAST(tw, Me)
                Return tw.GetResult()
            End Using
        End Function

    End Class
End Namespace