Namespace AST
    Public Class ReturnStatement
        Inherits StatementNode

        Private ReturnValue As AST.ExpressionNode

        Public Sub New(ReturnValue As AST.ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.ReturnValue = ReturnValue
        End Sub

        Public Overrides Sub Compile(Scope As Context.Scope)

            ' Get returnableScope
            Dim ReturnableScope As Context.ReturnableScope = Scope.GetParent(Of Context.ReturnableScope)
            If ReturnableScope Is Nothing Then
                Throw New SyntaxError("This scope does not allow you to return values.", Location)
            End If

            ' Check return type correspond to upper returnableSCope
            Dim ReturnType As TypeSystem.Type = ReturnValue.GetExpressionReturnType(Scope)
            ReturnableScope.DefineReturnType(ReturnType, Location)

            ' Compile
            Scope.WriteLine($"return {ReturnValue.CompileExpression(Scope)};")

        End Sub

    End Class

End Namespace