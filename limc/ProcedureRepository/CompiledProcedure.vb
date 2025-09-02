Public Interface CompiledProcedure

    ReadOnly Property Name As String
    ReadOnly Property ArgumentTypes As IEnumerable(Of TypeSystem.Type)
    Sub Compile()

End Interface
