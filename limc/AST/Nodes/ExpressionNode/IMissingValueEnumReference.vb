Namespace AST
    Public Interface IMissingValueEnumReference
        Function TryGetEnumType(Context As Context.Context) As TypeSystem.EnumType 'Can return null
        Function CompileEnumValue(Context As Context.Context, Writer As CWriter, Value As ExpressionNode) As String 'Can return empty string if no enum found
    End Interface
End Namespace