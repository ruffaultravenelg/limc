Namespace AST
    Public Class SourceStatement
        Inherits StatementNode

        Private Source As String

        Public Sub New(Source As String, Location As Location)
            MyBase.New(Location)
            Me.Source = Source
        End Sub

        Public Overrides Sub Compile(Writer As CWriter, Scope As Context.Scope)

            ' Regex to remplaces $word by values
            Dim result As String = CAPI.CompileSourceString(Source, Scope)

            'Write string directly to source file
            Writer.WriteLine(result)

        End Sub

    End Class
End Namespace