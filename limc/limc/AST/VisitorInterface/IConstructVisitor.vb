Namespace AST
    Friend Interface IConstructVisitor(Of D, R)
        Function VisitProgramNode(Node As ProgramNode, Data As D) As R
        Function VisitFileNode(Node As FileNode, Data As D) As R
        Function VisitFunctionConstruct(Node As FunctionConstruct, Data As D) As R
        Function VisitArgumentNode(Node As ArgumentNode, Data As D) As R
        Function VisitImportFileConstruct(Node As ImportFileConstruct, Data As D) As R
        Function VisitGenericTypeParameterNode(Node As GenericTypeParameterNode, Data As D) As R
    End Interface
End Namespace