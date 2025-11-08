Namespace TypeSystem
    Public Class BoolType
        Inherits Type

        Public Overrides ReadOnly Property cRepresentation As String = "bool"

        Public Overrides Function DefaultValue(Scope As Context.Scope) As String
            Return "false"
        End Function

        Public Overrides Function ToString() As String
            Return "bool"
        End Function

        Public Overrides Function RetrieveElements(Name As String) As IEnumerable(Of SearchMatch)
            Return {}
        End Function

        Private _Relations As New List(Of Lazy.Relation)
        Protected Overrides ReadOnly Property Relations As IEnumerable(Of Lazy.Relation)
            Get
                If _Relations.Count = 0 Then
                    CreateRelationHelper(RelationType.RELATION_AND, Type.Bool, Type.Bool, $"{INSTANCE_ARGUMENT_NAME} && val")
                    CreateRelationHelper(RelationType.RELATION_OR, Type.Bool, Type.Bool, $"{INSTANCE_ARGUMENT_NAME} || val")
                End If
                Return _Relations
            End Get
        End Property

        Private Sub CreateRelationHelper(RelationType As RelationType, ValType As Type, ReturnType As Type, ReturnValue As String)
            _Relations.Add(New Lazy.HardRelation(Me, RelationType, {ValType}, {"val"}, ReturnType, {$"return {ReturnValue};"}))
        End Sub

    End Class
End Namespace