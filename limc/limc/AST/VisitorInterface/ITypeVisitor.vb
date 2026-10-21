Namespace AST
    Friend Interface ITypeVisitor(Of D, R)

        Function VisitGenericTypeNode(Node As GenericTypeNode, Data As D) As R

    End Interface
End Namespace