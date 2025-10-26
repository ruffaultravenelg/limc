Namespace AST
    Public Class ReturnStatement
        Inherits StatementNode

        Private ReturnValue As AST.ExpressionNode

        Public Sub New(ReturnValue As AST.ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.ReturnValue = ReturnValue
        End Sub

        Public Overrides Sub Compile(Scope As Context.Scope)
            Throw New NotImplementedException()
        End Sub

    End Class

End Namespace