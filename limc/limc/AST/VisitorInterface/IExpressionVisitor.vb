Namespace AST
    Friend Interface IExpressionVisitor(Of D, R)
        Function VisitIntegerExpression(Node As IntegerExpression, Data As D) As R
        Function VisitFloatExpression(Node As FloatExpression, Data As D) As R
        Function VisitRawStringExpression(Node As RawStringExpression, Data As D) As R
        Function VisitFormattedStringExpression(Node As FormattedStringExpression, Data As D) As R
        Function VisitBooleanExpression(Node As BooleanExpression, Data As D) As R
    End Interface
End Namespace