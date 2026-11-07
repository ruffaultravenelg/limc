Imports limc.Symbol

Namespace HIR
    Friend Class FunctionReferenceExpression
        Inherits ExpressionNode

        Public ReadOnly Property FuncSymbol As FunctionSymbol

        Public Sub New(Location As Location, FuncSymbol As FunctionSymbol, FuncType As FuncTypeSymbol)
            MyBase.New(Location, FuncType)
            Me.FuncSymbol = FuncSymbol
        End Sub

        Public Overrides Sub Accept(Visitor As IHIRVisitor)
            Visitor.VisitFunctionReferenceExpression(Me)
        End Sub

    End Class
End Namespace