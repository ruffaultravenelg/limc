Namespace AST
    Public Class BreakStatement
        Inherits StatementNode

        Public Sub New(Location As Location)
            MyBase.New(Location)
        End Sub

        Public Overrides Sub Compile(Scope As Context.Scope)

            ' Check if this statement is in a loop
            Dim LoopScope As Context.LoopScope = Scope.GetParent(Of Context.LoopScope)
            If LoopScope Is Nothing Then
                Throw New SyntaxError("The ""break"" statement can only be used in a loop.", Location)
            End If

            ' Compile
            Scope.WriteLine("break;")

        End Sub

    End Class
End Namespace