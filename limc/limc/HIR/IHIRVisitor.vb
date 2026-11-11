Namespace HIR
    Friend Interface IHIRVisitor

        ' Constructs
        Sub VisitProgramNode(Node As ProgramNode)
        Sub VisitFunctionConstruct(Node As FunctionConstruct)
        Sub VisitArgumentNode(Node As ArgumentNode)

        ' Statements
        Sub VisitBlockStatmement(Node As BlockStatement)
        Sub VisitDeclareVariableStatement(Node As DeclareVariableStatement)
        Sub VisitProcedureCallStatement(Node As ProcedureCallStatement)
        Sub VisitVariableAssignStatement(Node As VariableAssignStatement)

    End Interface
End Namespace