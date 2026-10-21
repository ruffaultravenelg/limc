Namespace HIR
    Friend Interface IHIRVisitor

        Sub VisitProgramNode(Node As ProgramNode)
        Sub VisitFunctionConstruct(Node As FunctionConstruct)
        Sub VisitArgumentNode(Node As ArgumentNode)

        Sub VisitBlockStatmement(Node As BlockStatement)
        Sub VisitDeclareVariableStatement(Node As DeclareVariableStatement)

        Sub VisitIntNode(Node As IntNode)

    End Interface
End Namespace