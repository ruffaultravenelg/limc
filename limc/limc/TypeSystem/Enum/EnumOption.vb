Namespace TypeSystem

    Public MustInherit Class EnumOption

        Public MustOverride ReadOnly Property HasValue As Boolean

        Public MustOverride Function CompileValue() As String
        Public MustOverride Function CompileValue(Writer As CWriter, Context As Context.Context, Value As AST.ExpressionNode) As String

    End Class

End Namespace