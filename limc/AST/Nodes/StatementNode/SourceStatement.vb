Namespace AST
    Public Class SourceStatement
        Inherits StatementNode

        Private Source As String

        Public Sub New(Source As String, Location As Location)
            MyBase.New(Location)
            Me.Source = Source
        End Sub

        Public Overrides Sub Compile(Scope As Context.Scope)
            Scope.WriteLine(Source)
        End Sub

    End Class
End Namespace