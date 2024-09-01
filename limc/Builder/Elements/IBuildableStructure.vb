'
' Represents a C structure
'
Public Interface IBuildableStructure

    Function BuildTypeForward() As String
    Function BuildStructureDefinition() As IEnumerable(Of String)

End Interface
