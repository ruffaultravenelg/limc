Namespace AST
    Public Class WhileStatement
        Inherits StatementNode

        Private Condition As ExpressionNode
        Private Instructions As IEnumerable(Of StatementNode)

        Public Sub New(Condition As ExpressionNode, Instructions As IEnumerable(Of StatementNode), Location As Location)
            MyBase.New(Location)
            Me.Condition = Condition
            Me.Instructions = Instructions
        End Sub

        Public Overrides Sub Compile(Scope As Context.Scope)

            ' Check condition type
            Dim ConditionType As TypeSystem.Type = Condition.GetExpressionReturnType(Scope)
            If ConditionType IsNot TypeSystem.Type.Bool Then
                Throw New TypeMismatchError(TypeSystem.Type.Bool, ConditionType, Condition.Location)
            End If

            ' Compile body
            Dim BodyContext As New Context.LoopScope(Scope, Location)
            For Each Statement In Instructions
                Statement.Compile(BodyContext)
            Next

            ' Compile whole loop
            Scope.WriteLine($"while ({Condition.CompileExpression(Scope)}){{")
            Scope.WriteScope(BodyContext)
            Scope.WriteLine("}")

        End Sub

        Protected Overrides Function GetChildNodes() As IEnumerable(Of StatementNode)
            Return Instructions
        End Function

    End Class
End Namespace