Namespace TypeSystem
    Public MustInherit Class EnumType
        Inherits CommonType

        Public Sub New(Name As String, GenericTypes As IEnumerable(Of TypeSystem.Type), ConstructIsExported As Boolean)
            MyBase.New(Name, GenericTypes, ConstructIsExported)
        End Sub

        Public MustOverride Function GetOptionValueByName(Name As String) As EnumOption ' can return nothing if not found

    End Class
End Namespace