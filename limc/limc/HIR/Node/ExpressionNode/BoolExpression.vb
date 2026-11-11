Namespace HIR
    Friend Class BoolExpression
        Inherits ExpressionNode

        Public ReadOnly Property Value As Boolean

        Public Sub New(Location As Location, Value As Boolean, BoolSymbType As Symbol.BoolTypeSymbol)
            MyBase.New(Location, BoolSymbType)
            Me.Value = Value
        End Sub

        Public Overrides Function Accept(Of R)(Visitor As IHIRExpressionVisitor(Of R)) As R
            Return Visitor.VisitBoolExpression(Me)
        End Function

    End Class
End Namespace