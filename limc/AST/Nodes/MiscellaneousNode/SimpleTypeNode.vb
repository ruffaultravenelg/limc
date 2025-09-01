Namespace AST
    Public Class SimpleTypeNode
        Inherits AST.TypeNode

        Private ReadOnly TypeName As String
        Private ReadOnly TypeGenericTypes As IEnumerable(Of AST.TypeNode)

        Public Sub New(TypeName As String, TypeGenericTypes As IEnumerable(Of AST.TypeNode), Location As Location)
            MyBase.New(Location)
            Me.TypeName = TypeName
            Me.TypeGenericTypes = TypeGenericTypes
        End Sub

        Public Overrides Function GetAssociatedType(Context As Context) As TypeSystem.Type
            Throw New NotImplementedException()
        End Function

    End Class

End Namespace