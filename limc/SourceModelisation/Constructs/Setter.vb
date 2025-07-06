Namespace Source
    Public Class Setter
        Inherits ConstructNode

        'Properties
        Public ReadOnly Property Name As String
        Public ReadOnly Property ValueVariableName As String
        Public ReadOnly Property ValueType As Source.Type
        Public ReadOnly Property Body As IEnumerable(Of StatementNode)

        'Constructor
        Public Sub New(Location As Location, Name As String, ValueVariableName As String, ValueType As Source.Type, Body As IEnumerable(Of StatementNode))
            MyBase.New(Location)
            Me.Name = Name
            Me.ValueVariableName = ValueVariableName
            Me.ValueType = ValueType
            Me.Body = Body
        End Sub

    End Class
End Namespace