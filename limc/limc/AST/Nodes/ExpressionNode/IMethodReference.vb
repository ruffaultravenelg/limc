Namespace AST
    Public Interface IMethodReference
        Function TryGetReferencedMethod(Context As Context.Context) As Lazy.Method
        Function GetCompiledInstance(Writer As CWriter, Scope As Context.Scope) As String
    End Interface
End Namespace