Namespace AST
    Public Class SourceStatementNode
        Inherits StatementNode

        Private Source As String

        Public Sub New(Source As String, Location As Location)
            MyBase.New(Location)
            Me.Source = Source
        End Sub

    End Class
End Namespace