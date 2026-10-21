Imports limc.Symbol

Namespace HIR
    Friend Class DeclareVariableStatement
        Inherits StatementNode

        Public ReadOnly Property VariableSymbol As VariableSymbol
        Public ReadOnly Property Value As ExpressionNode

        Public Sub New(Location As Location, VariableSymbol As VariableSymbol, Value As ExpressionNode)
            MyBase.New(Location)
            Me.VariableSymbol = VariableSymbol
            Me.Value = Value
        End Sub
        Public Overrides Sub Accept(Visitor As IHIRVisitor)
            Visitor.VisitDeclareVariableStatement(Me)
        End Sub

    End Class
End Namespace