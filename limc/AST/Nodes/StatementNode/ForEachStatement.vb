Namespace AST
    Public Class ForEachStatement
        Inherits StatementNode

        Private VariableName As String
        Private VariableType As TypeNode
        Private Sequence As ExpressionNode
        Private Instructions As IEnumerable(Of StatementNode)

        Public Sub New(VariableName As String, VariableType As TypeNode, Sequence As ExpressionNode, Instructions As IEnumerable(Of StatementNode), Location As Location)
            MyBase.New(Location)
            Me.VariableName = VariableName
            Me.VariableType = VariableType
            Me.Sequence = Sequence
            Me.Instructions = Instructions
        End Sub

        Public Overrides Sub Compile(Writer As CWriter, Scope As Context.Scope)
            Throw New NotImplementedException()
        End Sub

        Protected Overrides Function GetChildNodes() As IEnumerable(Of StatementNode)
            Return Instructions
        End Function

    End Class
End Namespace