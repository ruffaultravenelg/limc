Public Interface IGenerateType
    Function InstanciateType(GenericTypes As IEnumerable(Of TypeSystem.Type)) As TypeSystem.Type
    Function DoMatchTypeInfo(Name As String, GenericTypes As IEnumerable(Of TypeSystem.Type)) As Boolean
End Interface
