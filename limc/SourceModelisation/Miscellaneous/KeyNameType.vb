Namespace Source

    Public Class KeyNameType
        Inherits Node

        'Properties
        Public ReadOnly Property Name As String
        Public ReadOnly Property Type As Source.Type

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

    End Class

End Namespace