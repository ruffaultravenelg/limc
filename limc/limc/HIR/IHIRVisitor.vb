Namespace HIR
    Friend Interface IHIRVisitor

        Sub VisitProgramNode(Node As ProgramNode)
        Sub VisitFunctionConstruct(Node As FunctionConstruct)
        Sub VisitArgumentNode(Node As ArgumentNode)

        Sub VisitBlockStatmement(Node As BlockStatement)
        Sub VisitDeclareVariableStatement(Node As DeclareVariableStatement)

        Sub VisitIntExpression(Node As IntExpression)
        Sub VisitFloatExpression(Node As FloatExpression)
        Sub VisitBoolExpression(Node As BoolExpression)
        Sub VisitRawStrExpression(Node As RawStrExpression)
        Sub VisitFormattedStrExpression(Node As FormattedStrExpression)
        Sub VisitProcedureCallExpression(Node As ProcedureCallExpression)
        Sub VisitVariableExpression(Node As VariableExpression)
        Sub VisitFunctionReferenceExpression(Node As FunctionReferenceExpression)

    End Interface
End Namespace