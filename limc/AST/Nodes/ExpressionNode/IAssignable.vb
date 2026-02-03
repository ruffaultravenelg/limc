Namespace AST
    Public Interface IAssignable
        Sub CompileAssignation(NewValue As ExpressionNode, Writer As CWriter, Scope As Context.Scope)
    End Interface
End Namespace