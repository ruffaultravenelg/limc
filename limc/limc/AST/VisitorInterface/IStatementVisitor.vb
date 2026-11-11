Namespace AST
    Friend Interface IStatementVisitor(Of D, R)
        Function VisitDeclareVariableInferTypeStatement(Node As DeclareVariableInferTypeStatement, Data As D) As R
        Function VisitBlockStatement(Node As BlockStatement, Data As D) As R
        Function VisitProcedureCallStatement(Node As ProcedureCallStatement, Data As D) As R
        Function VisitVariableAssignStatement(Node As VariableAssignStatement, Data As D) As R
        Function VisitReturnStatement(Node As ReturnStatement, Data As D) As R
    End Interface
End Namespace