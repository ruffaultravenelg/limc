Namespace AST
    Public Interface IAssignable
        Sub CompileAssignation(NewValue As ExpressionNode, Scope As Context.Scope)
    End Interface
End Namespace