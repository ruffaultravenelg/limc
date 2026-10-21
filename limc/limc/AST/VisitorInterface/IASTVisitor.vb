Namespace AST
    ' D: Data <=> Argument type
    ' R: Return <=> Return type
    Friend Interface IASTVisitor(Of D, R)
        Inherits IConstructVisitor(Of D, R)
        Inherits IStatementVisitor(Of D, R)
        Inherits IExpressionVisitor(Of D, R)
        Inherits ITypeVisitor(Of D, R)

    End Interface
End Namespace