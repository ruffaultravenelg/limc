
Namespace Source

    Public Class Struct
        Inherits TypeConstruct

        'Properties
        Public Overrides ReadOnly Property Name As String
        Public ReadOnly Property Properties As IEnumerable(Of Source.KeyNameType)
        Public Overrides ReadOnly Property GenericTypes As IEnumerable(Of GenericType)

        'Constructor
        Public Sub New(Location As Location, Name As String, GenericTypes As IEnumerable(Of GenericType), Properties As IEnumerable(Of Source.KeyNameType))
            MyBase.New(Location)
            Me.Name = Name
            Me.GenericTypes = GenericTypes
            Me.Properties = Properties
        End Sub

        'To string
        Public Overrides Function ToString() As String
            Return "struct " & Name & GenericType.ListToString(GenericTypes) & Source.KeyNameType.ListToString(Properties)
        End Function

        Public Overrides Function Compile(PassedGenericTypes As IEnumerable(Of Lim.Type)) As Lim.Type
            Return New Lim.StructType(Me, PassedGenericTypes)
        End Function
    End Class

End Namespace