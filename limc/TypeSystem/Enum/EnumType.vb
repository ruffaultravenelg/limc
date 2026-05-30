Imports limc.AST

Namespace TypeSystem
    Public MustInherit Class EnumType
        Inherits CommonType

        Public Sub New(Name As String, GenericTypes As IEnumerable(Of TypeSystem.Type), ConstructIsExported As Boolean)
            MyBase.New(Name, GenericTypes, ConstructIsExported)
        End Sub

        Public Function GetOptionValueByName(Name As String) As EnumOption
            Return Options.FirstOrDefault(Function(o) o.Name = Name)
        End Function

        Protected Options As List(Of EnumOption)



    End Class
End Namespace