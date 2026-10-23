Imports limc.Environment
Imports limc.Symbol

Namespace HIR
    Friend Class FunctionReferenceExpression
        Inherits ExpressionNode

        Public ReadOnly Property FuncSymbol As FunctionSymbol

        Public Sub New(Location As Location, FuncSymbol As FunctionSymbol, FuncType As FuncTypeSymbol)
            MyBase.New(Location, FuncType)
            Me.FuncSymbol = FuncSymbol
        End Sub
        Public Sub New(Location As Location, FuncSymbol As FunctionSymbol)
            Me.New(Location, FuncSymbol, Primitives.Func(FuncSymbol.Arguments.Select(Function(arg) arg.VariableType), FuncSymbol.ReturnType))
        End Sub

        Public Overrides Sub Accept(Visitor As IHIRVisitor)
            Visitor.VisitFunctionReferenceExpression(Me)
        End Sub

    End Class
End Namespace