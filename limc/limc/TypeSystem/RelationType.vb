Namespace TypeSystem
    Public Enum RelationType
        RELATION_BRACKETS
        RELATION_ADD
        RELATION_SUB
        RELATION_MULT
        RELATION_DIV
        RELATION_MODULO
        RELATION_UNARY_MINUS
        RELATION_SET_BRACKETS
        RELATION_EQUAL
        RELATION_GREATERTHAN
        RELATION_GREATERTHANEQUAL
        RELATION_LESSTHAN
        RELATION_LESSTHANEQUAL
        RELATION_BRACKETS_PTR
    End Enum

    Module RelationUtil

        Public Function RelationUseSelf(RelationType As RelationType)
            Select Case RelationType
                Case RelationType.RELATION_ADD, RelationType.RELATION_SUB, RelationType.RELATION_MULT, RelationType.RELATION_DIV, RelationType.RELATION_MODULO,
                     RelationType.RELATION_LESSTHAN, RelationType.RELATION_LESSTHANEQUAL, RelationType.RELATION_GREATERTHAN, RelationType.RELATION_GREATERTHANEQUAL,
                     RelationType.RELATION_EQUAL, RelationType.RELATION_UNARY_MINUS
                    Return False

                Case Else
                    Return True
            End Select
        End Function

    End Module

End Namespace