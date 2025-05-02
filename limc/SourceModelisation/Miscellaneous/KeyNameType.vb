Namespace Source

    Public Class KeyNameType
        Inherits Node
        Implements IPropertieDefinition 'For structure & class definition

        'Properties
        Public ReadOnly Property Name As String Implements IPropertieDefinition.Name
        Public ReadOnly Property Type As Source.Type Implements IPropertieDefinition.Type

        'Constructor
        Public Sub New(Location As Location, Name As String, Type As Source.Type)
            MyBase.New(Location)
            Me.Name = Name
            Me.Type = Type
        End Sub

        'To string
        Public Overrides Function ToString() As String
            Return Name & ":" & Type.ToString()
        End Function

        'List of argument
        Public Shared Function ListToString(List As IEnumerable(Of Source.KeyNameType)) As String
            Return "(" & String.Join(", ", List.Select(Function(Argument) Argument.ToString())) & ")"
        End Function

        'For struct / classs inline propertie definition
        Public ReadOnly Property [GET] As Boolean = True Implements IPropertieDefinition.GET
        Public ReadOnly Property [SET] As Boolean = True Implements IPropertieDefinition.SET

    End Class

End Namespace