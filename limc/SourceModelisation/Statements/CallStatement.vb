Namespace Source

    Public Class CallStatement
        Inherits StatementNode

        'Properties
        Private Expression As Source.CallNode

        'Constructor
        Public Sub New(Expression As Source.CallNode)
            MyBase.New(Expression.Location)
            Me.Expression = Expression
        End Sub

        'Compile
        Public Overrides Sub Compile(Scope As Scope)
            Scope.WriteLine(Expression.Compile(Scope, False) & ";")
        End Sub

        'Contains return statement
        Public Overrides ReadOnly Property ContainsReturnStatement As Boolean = False

    End Class

End Namespace