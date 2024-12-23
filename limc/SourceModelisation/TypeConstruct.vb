'Represent a base for a type
Public MustInherit Class TypeConstruct
    Inherits ConstructNode

    'Constructor
    Public Sub New(Location As Location)
        MyBase.New(Location)
    End Sub

    'Generic types
    Public MustOverride ReadOnly Property GenericTypes As IEnumerable(Of Source.GenericType)

    'Name
    Public MustOverride ReadOnly Property Name As String

    'Compile
    Public MustOverride Function Compile(PassedGenericTypes As IEnumerable(Of Lim.Type)) As Lim.Type

End Class
