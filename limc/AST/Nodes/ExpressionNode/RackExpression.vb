Namespace AST
    Public Class RackExpression
        Inherits ExpressionNode

        Private Elements As IEnumerable(Of ExpressionNode)

        Public Sub New(Elements As IEnumerable(Of ExpressionNode), Location As Location)
            MyBase.New(Location)
            Me.Elements = Elements
            If Elements.Count < 1 Then
                Throw New SyntaxError("A rack must contain at least one item in order to retrieve the rack type.", Location)
            End If
        End Sub

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type
            Return TypeSystem.RackType.FromLengthAndType(Elements.Count, Elements.First.GetExpressionReturnType(Context))
        End Function

        Public Overrides Function CompileExpression(Scope As Context.Scope) As String

            Dim FirstType As TypeSystem.Type = Elements.First.GetExpressionReturnType(Scope)

            Dim CompiledElements As New List(Of String)
            For Each Elm As ExpressionNode In Elements
                If Elm.GetExpressionReturnType(Scope) <> FirstType Then
                    Throw New TypeMismatchError(FirstType, Elm.GetExpressionReturnType(Scope), Elm.Location)
                End If
                CompiledElements.Add(Elm.CompileExpression(Scope))
            Next

            Return "{" & String.Join(", ", CompiledElements) & "}"
        End Function

    End Class
End Namespace