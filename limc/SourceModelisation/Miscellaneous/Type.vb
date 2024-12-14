Namespace Source

    'Basic TypeNode -> "list<str>"
    Public Class Type
        Inherits Node

        'Properties
        Protected ReadOnly TypeName As String
        Protected ReadOnly PassedGenericTypes As IEnumerable(Of Source.Type)

        'Constructor
        Public Sub New(Location As Location, TypeName As String, PassedGenericTypes As IEnumerable(Of Source.Type))
            MyBase.New(Location)
            Me.TypeName = TypeName
            Me.PassedGenericTypes = PassedGenericTypes
        End Sub

        'Get targeted type
        Public Overridable Function GetTargetedType(Scope As Object) As Lim.Type
            Throw New NotImplementedException
        End Function

    End Class

    'TypeNode from another file -> "utils::linkedList<str>"
    Public Class FiledType
        Inherits Type

        'Propertie
        Protected ReadOnly Filename As String

        'Constructor
        Public Sub New(Location As Location, Filename As String, TypeName As String, PassedGenericTypes As IEnumerable(Of Source.Type))
            MyBase.New(Location, TypeName, PassedGenericTypes)
            Me.Filename = Filename
        End Sub

        'Get targeted type
        Public Overrides Function GetTargetedType(Scope As Object) As Lim.Type
            Throw New NotImplementedException
        End Function

    End Class

End Namespace