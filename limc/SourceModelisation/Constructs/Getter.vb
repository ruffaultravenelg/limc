Namespace Source
    Public Class Getter
        Inherits ConstructNode

        'Properties
        Public ReadOnly Property Name As String
        Public ReadOnly Property DefinedType As Source.Type
        Public ReadOnly Property Body As IEnumerable(Of StatementNode)

        'Constructor
        Public Sub New(Location As Location, Name As String, DefinedType As Source.Type, Body As IEnumerable(Of StatementNode))
            MyBase.New(Location)
            Me.Name = Name
            Me.DefinedType = DefinedType
            Me.Body = Body
        End Sub

    End Class
End Namespace