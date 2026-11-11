Namespace HIR
    Friend Interface IHIRExpressionVisitor(Of R)

        ' Expression
        Function VisitIntExpression(Node As IntExpression) As R
        Function VisitFloatExpression(Node As FloatExpression) As R
        Function VisitBoolExpression(Node As BoolExpression) As R
        Function VisitRawStrExpression(Node As RawStrExpression) As R
        Function VisitFormattedStrExpression(Node As FormattedStrExpression) As R
        Function VisitProcedureCallExpression(Node As ProcedureCallExpression) As R
        Function VisitVariableExpression(Node As VariableExpression) As R
        Function VisitFunctionReferenceExpression(Node As FunctionReferenceExpression) As R
        Function VisitMethodReferenceExpression(Node As ObjMethodReferenceExpression) As R

    End Interface
End Namespace