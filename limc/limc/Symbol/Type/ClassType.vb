Namespace Symbol
    Friend Class ClassType
        Inherits TypeSymbol

        Public ReadOnly Property Node As AST.ClassConstruct

        Public Overrides ReadOnly Property IS_POINTER As Boolean = True

        Public Sub New(Node As AST.ClassConstruct, Env As Environment.Env)
            MyBase.New(Node.ClassName, Env)
            Me.Node = Node
        End Sub

        Public Overrides Sub Compile(Lowerer As HIR.Lowerer.IHIRLowerer, ProgramBuilder As HIR.ProgramNode.Builder)
            Throw New NotImplementedException()
        End Sub

        Public Overrides Function ToString() As String
            Throw New NotImplementedException()
        End Function

    End Class
End Namespace