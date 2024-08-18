'
' Represents a C structure
'
Public Interface BuildableStructure

    Function BuildTypeForward() As String
    Function BuildStructureDefinition() As IEnumerable(Of String)

End Interface
