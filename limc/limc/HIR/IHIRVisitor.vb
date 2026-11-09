Namespace HIR
    Friend Interface IHIRVisitor

        Sub VisitProgramNode(Node As ProgramNode)
        Sub VisitFunctionConstruct(Node As FunctionConstruct)
        Sub VisitArgumentNode(Node As ArgumentNode)

        Sub VisitBlockStatmement(Node As BlockStatement)
        Sub VisitDeclareVariableStatement(Node As DeclareVariableStatement)
        Sub VisitProcedureCallStatement(Node As ProcedureCallStatement)
        Sub VisitVariableAssignStatement(Node As VariableAssignStatement)

        Sub VisitIntExpression(Node As IntExpression)
        Sub VisitFloatExpression(Node As FloatExpression)
        Sub VisitBoolExpression(Node As BoolExpression)
        Sub VisitRawStrExpression(Node As RawStrExpression)
        Sub VisitFormattedStrExpression(Node As FormattedStrExpression)
        Sub VisitProcedureCallExpression(Node As ProcedureCallExpression)
        Sub VisitVariableExpression(Node As VariableExpression)
        Sub VisitFunctionReferenceExpression(Node As FunctionReferenceExpression)
        Sub VisitMethodReferenceExpression(Node As ObjMethodReferenceExpression)

    End Interface
End Namespace