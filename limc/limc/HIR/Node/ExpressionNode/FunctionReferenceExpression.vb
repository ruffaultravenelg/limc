Imports limc.Symbol

Namespace HIR
    Friend Class FunctionReferenceExpression
        Inherits ExpressionNode

        Public ReadOnly Property FuncSymbol As FunctionSymbol

        Public Sub New(Location As Location, FuncSymbol As FunctionSymbol, FuncType As FuncTypeSymbol)
            MyBase.New(Location, FuncType)
            Me.FuncSymbol = FuncSymbol
        End Sub

        Public Overrides Function Accept(Of R)(Visitor As IHIRExpressionVisitor(Of R)) As R
            Return Visitor.VisitFunctionReferenceExpression(Me)
        End Function

    End Class
End Namespace