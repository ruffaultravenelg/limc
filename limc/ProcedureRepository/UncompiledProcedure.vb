Public Interface UncompiledProcedure

    ReadOnly Property Name As String
    Function GetArgumentTypes(CompilingContext As Context) As IEnumerable(Of TypeSystem.Type)
    Function CompileProcedure(CompilingContext As Context) As CompiledProcedure

End Interface
