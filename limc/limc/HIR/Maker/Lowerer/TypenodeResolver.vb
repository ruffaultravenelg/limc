Imports limc.Symbol

Namespace HIR.Lowerer
    Friend Class TypenodeResolver
        Implements AST.ITypeVisitor(Of Environment.Env, TypeSymbol)

        Private Shared ReadOnly Singleton As New TypenodeResolver()

        ' ==== ACCESS FUNCTION ====
        Public Shared Function Resolve(Node As AST.TypeNode, Env As Environment.Env) As TypeSymbol
            Dim type = Node.Accept(Singleton, Env)
            If type IsNot Nothing Then
                Return type
            Else
                Throw New UnreachableElementException("HTR01", Node.ToString(), Node.Location)
            End If
        End Function


        ' ==== VISITOR ====

        Private Function VisitGenericTypeNode(Node As AST.GenericTypeNode, Env As Environment.Env) As TypeSymbol Implements AST.ITypeVisitor(Of Environment.Env, TypeSymbol).VisitGenericTypeNode
            Return Env.FindType(Node.Name, Node.Arguments.Select(Function(t) Resolve(t, Env)))
        End Function

    End Class
End Namespace