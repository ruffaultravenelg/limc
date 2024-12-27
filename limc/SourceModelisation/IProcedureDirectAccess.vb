Public Interface IProcedureDirectAccess

    Function GetProcedureReturnedType(Context As Context, PassedArguments As IEnumerable(Of ExpressionNode)) As Lim.Type

    Function CompileProcedureCall(Context As Context, Passedarguments As IEnumerable(Of ExpressionNode)) As String

End Interface
