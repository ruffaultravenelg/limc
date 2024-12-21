Namespace Source

    Public Class GenericType
        Inherits Node

        'Properties
        Public ReadOnly Property Name As String

        'Constructor
        Public Sub New(Location As Location, Name As String)
            MyBase.New(Location)
            Me.Name = Name
        End Sub

        'To string
        Public Overrides Function ToString() As String
            Return Name
        End Function

        'List to string
        Public Shared Function ListToString(List As IEnumerable(Of Source.GenericType)) As String
            Return "<" & String.Join(", ", List.Select(Function(GenericType) GenericType.ToString())) & ">"
        End Function

    End Class

End Namespace