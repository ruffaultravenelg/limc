'
' Allows a procedure selector node to implement code that allows direct function calls in the source code, instead of first encapsulating it in a function pointer.
'
Public Interface ProcedureSelectorNode

    ' Allows you to search for a procedure with or without the help of given arguments, returns Nothing if no unique function is found.
    Function GetProcedureByYourself(Scope As Scope) As ICompiledProcedure
    Function GetProcedureWithHelpOfArgs(Scope As Scope, ProvidedArguments As IEnumerable(Of ExpressionNode)) As ICompiledProcedure

    ' Compile a call to a given procedure
    Function CompileCallTo(Procedure As ICompiledProcedure, Scope As Scope, ProvidedArguments As IEnumerable(Of ExpressionNode)) As String

End Interface
