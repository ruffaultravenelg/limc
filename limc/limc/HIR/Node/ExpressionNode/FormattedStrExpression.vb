Imports limc.Environment

Namespace HIR
    Friend Class FormattedStrExpression
        Inherits ExpressionNode

        Public ReadOnly Property Value As String

        Public Sub New(Location As Location, Value As String, StrTypeSymb As Symbol.StrTypeSymbol)
            MyBase.New(Location, StrTypeSymb)
            Me.Value = Value
        End Sub

        Public Overrides Function Accept(Of R)(Visitor As IHIRExpressionVisitor(Of R)) As R
            Return Visitor.VisitFormattedStrExpression(Me)
        End Function

    End Class
End Namespace