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

            'Integrated type
            Select Case TypeName
                Case "int"
                    NoGenericType("int")
                    Return TypeSystem.Type.Int
            End Select

            'Search for generic types

            'Search for type
            Throw New NotImplementedException()

        End Function

        Private Sub NoGenericType(TypeName As String)
            If TypeGenericTypes.Any() Then
                Throw New TypeError($"Type '{TypeName}' does not support generic types", Location)
            End If
        End Sub

    End Class

End Namespace