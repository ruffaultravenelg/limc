Namespace AST
    Public Class IfStatement
        Inherits StatementNode

        Private MainCondition As ExpressionNode
        Private MainInstructions As IEnumerable(Of StatementNode)
        Private ElseIfBlocks As IEnumerable(Of ElseifBlock)
        Private ElseInstructions As IEnumerable(Of StatementNode)

        Public Sub New(MainCondition As ExpressionNode, MainInstructions As IEnumerable(Of StatementNode), ElseIfBlocks As IEnumerable(Of ElseifBlock), ElseInstructions As IEnumerable(Of StatementNode), Location As Location)
            MyBase.New(Location)
            Me.MainCondition = MainCondition
            Me.MainInstructions = MainInstructions
            Me.ElseIfBlocks = ElseIfBlocks
            Me.ElseInstructions = ElseInstructions
        End Sub

        Public Overrides Sub Compile(Scope As Context.Scope)

            ' Check main condition type
            Dim ConditionType As TypeSystem.Type = MainCondition.GetExpressionReturnType(Scope)
            If ConditionType IsNot TypeSystem.Type.Bool Then
                Throw New TypeMismatchError(TypeSystem.Type.Bool, ConditionType, MainCondition.Location)
            End If

            ' Compile main
            Dim MainBodyScope As New Context.Scope(Scope, Location)
            For Each Statement In MainInstructions
                Statement.Compile(MainBodyScope)
            Next
            Scope.WriteLine($"if ({MainCondition.CompileExpression(Scope)}){{")
            Scope.WriteScope(MainBodyScope)
            Scope.WriteLine("}")

            ' Compile elseifs
            For Each ElseIfBlock In ElseIfBlocks

                ' Check block's condition type
                Dim BlockConditionType As TypeSystem.Type = ElseIfBlock.Condition.GetExpressionReturnType(Scope)
                If BlockConditionType IsNot TypeSystem.Type.Bool Then
                    Throw New TypeMismatchError(TypeSystem.Type.Bool, BlockConditionType, ElseIfBlock.Condition.Location)
                End If

                'Compile block
                Dim BlockBodyScope As New Context.Scope(Scope, Location)
                For Each Statement In ElseIfBlock.Body
                    Statement.Compile(BlockBodyScope)
                Next
                Scope.AppendLastLine($" else if ({ElseIfBlock.Condition.CompileExpression(Scope)}){{")
                Scope.WriteScope(BlockBodyScope)
                Scope.WriteLine("}")

            Next

            ' Compile else
            If ElseInstructions IsNot Nothing Then
                Dim ElseBodyScope As New Context.Scope(Scope, Location)
                For Each Statement In ElseInstructions
                    Statement.Compile(ElseBodyScope)
                Next
                Scope.AppendLastLine(" else {")
                Scope.WriteScope(ElseBodyScope)
                Scope.WriteLine("}")
            End If

        End Sub

        Protected Overrides Function GetChildNodes() As IEnumerable(Of StatementNode)
            Dim Childs As New List(Of StatementNode)
            Childs.AddRange(MainInstructions)
            For Each Block In ElseIfBlocks
                Childs.AddRange(Block.Body)
            Next
            If ElseInstructions IsNot Nothing Then
                Childs.AddRange(ElseInstructions)
            End If
            Return Childs
        End Function

        Public Class ElseifBlock
            Public ReadOnly Property Condition As ExpressionNode
            Public ReadOnly Property Body As IEnumerable(Of StatementNode)
            Public Sub New(Condition As ExpressionNode, Body As IEnumerable(Of StatementNode))
                Me.Condition = Condition
                Me.Body = Body
            End Sub
        End Class

    End Class
End Namespace