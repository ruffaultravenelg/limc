Namespace AST
    Public Class RecordConstruct
        Inherits ConstructNode
        Implements IGenerateType

        Public ReadOnly Property Name As String
        Public ReadOnly Property Fields As IEnumerable(Of Field)
        Public ReadOnly Property GenericTypeNames As IEnumerable(Of String)

        Public Sub New(Name As String, GenericTypeNames As IEnumerable(Of String), Fields As IEnumerable(Of Field), Location As Location)
            MyBase.New(Location)
            Me.Name = Name
            Me.GenericTypeNames = GenericTypeNames
            Me.Fields = Fields
        End Sub

        Public Function InstanciateType(GenericTypes As IEnumerable(Of TypeSystem.Type)) As TypeSystem.Type Implements IGenerateType.InstanciateType
            Return New TypeSystem.RecordType(Me, GenericTypes)
        End Function

        Public Function DoMatchTypeInfo(Name As String, GenericTypes As IEnumerable(Of TypeSystem.Type)) As Boolean Implements IGenerateType.DoMatchTypeInfo
            Return Name = Me.Name AndAlso GenericTypes.Count = GenericTypeNames.Count
        End Function

        Public Class Field
            Public ReadOnly Property Name As String
            Public ReadOnly Property Type As TypeNode
            Public ReadOnly Property Location As Location

            Public Sub New(Name As String, Type As TypeNode, Location As Location)
                Me.Name = Name
                Me.Type = Type
                Me.Location = Location
            End Sub

        End Class

    End Class
End Namespace