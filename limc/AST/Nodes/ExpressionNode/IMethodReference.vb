Namespace AST
    Public Interface IMethodReference
        Function TryGetReferencedMethod(Context As Context.Context) As Lazy.Method
        Function GetInstanceExpression() As ExpressionNode
    End Interface
End Namespace