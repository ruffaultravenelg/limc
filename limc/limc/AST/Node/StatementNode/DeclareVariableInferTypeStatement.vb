Namespace AST
    Friend Class DeclareVariableInferTypeStatement
        Inherits StatementNode

        Public ReadOnly Property VariableName As String
        Public ReadOnly Property Value As ExpressionNode

        Public Sub New(VariableName As String, Value As ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.VariableName = VariableName
            Me.Value = Value
        End Sub

        Public Overloads Overrides Function Accept(Of D, R)(Visitor As IStatementVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitDeclareVariableInferTypeStatement(Me, Data)
        End Function

    End Class
End Namespace