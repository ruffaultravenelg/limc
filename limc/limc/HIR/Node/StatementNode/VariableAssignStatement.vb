Imports limc.Symbol

Namespace HIR
    Friend Class VariableAssignStatement
        Inherits StatementNode

        Public ReadOnly Property Variable As VariableSymbol
        Public ReadOnly Property NewValue As ExpressionNode

        Public Sub New(Location As Location, Variable As VariableSymbol, NewValue As ExpressionNode)
            MyBase.New(Location)
            Me.Variable = Variable
            Me.NewValue = NewValue
        End Sub

        Public Overrides Sub Accept(Visitor As IHIRVisitor)
            Visitor.VisitVariableAssignStatement(Me)
        End Sub

    End Class
End Namespace