Namespace AST
    Public Interface IFunctionReference
        Function TryGetReferencedFunction(Context As Context.Context) As Lazy.Function
    End Interface
End Namespace