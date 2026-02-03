Namespace AST
    Public Class ReturnStatement
        Inherits StatementNode

        Private ReturnValue As AST.ExpressionNode

        Public Sub New(ReturnValue As AST.ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.ReturnValue = ReturnValue
        End Sub

        Public Overrides Sub Compile(Writer As CWriter, Scope As Context.Scope)

            ' Get returnableScope
            Dim ReturnableScope As Context.MustReturnScope = Scope.GetParent(Of Context.MustReturnScope)
            If ReturnableScope Is Nothing Then
                Throw New SyntaxError("This scope does not allow you to return values.", Location)
            End If

            ' Check return type correspond to upper returnableSCope
            Dim ReturnType As TypeSystem.Type = ReturnValue.GetExpressionReturnType(Scope)
            If Not ReturnType = ReturnableScope.ReturnType Then
                Throw New TypeMismatchError(ReturnableScope.ReturnType, ReturnType, Location)
            End If

            ' Compile
            Writer.WriteLine($"return {ReturnValue.CompileExpression(Writer, Scope)};")

        End Sub

    End Class

End Namespace