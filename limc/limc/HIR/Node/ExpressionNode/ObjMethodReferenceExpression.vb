Namespace HIR
    Friend Class ObjMethodReferenceExpression
        Inherits ExpressionNode

        Public ReadOnly Property Element As ExpressionNode
        Public ReadOnly Property Method As Symbol.MethodSymbol

        Public Sub New(Location As Location, Element As ExpressionNode, Method As Symbol.MethodSymbol, MethodFuncType As Symbol.FuncTypeSymbol)
            MyBase.New(Location, MethodFuncType)
            Me.Element = Element
            Me.Method = Method
        End Sub

        Public Overrides Sub Accept(Visitor As IHIRVisitor)
            Visitor.VisitMethodReferenceExpression(Me)
        End Sub

    End Class
End Namespace