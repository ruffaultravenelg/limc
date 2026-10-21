Namespace AST
    Friend Interface IStatementVisitor(Of D, R)
        Function VisitDeclareVariableInferTypeStatement(Node As DeclareVariableInferTypeStatement, Data As D) As R
        Function VisitBlockStatement(Node As BlockStatement, Data As D) As R
    End Interface
End Namespace