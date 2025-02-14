Public Interface IProcedureDirectAccess

    Function GetProcedureReturnedType(Context As Context, PassedArguments As IEnumerable(Of Lim.Type)) As Lim.Type

    Function CompileProcedureCall(Scope As Scope, Passedarguments As IEnumerable(Of ExpressionNode)) As String

End Interface
