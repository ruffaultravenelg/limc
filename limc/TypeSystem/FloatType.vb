Namespace TypeSystem
    Public Class FloatType
        Inherits Type

        Public Overrides ReadOnly Property cRepresentation As String = "double"

        Public Overrides Function DefaultValue(Scope As Context.Scope) As String
            Return "0.0"
        End Function

        Public Overrides Function ToString() As String
            Return "float"
        End Function

        Public Sub Compile()
            RegisterMethod(New Lazy.HardMethod(Me, "str", {}, Type.Str, {
                $"char* buffer = {LIM_ALLOC}({FLOAT_TO_STR_BUFFERSIZE});",
                $"sprintf(buffer, ""%f"", {INSTANCE_ARGUMENT_NAME});",
                "return buffer;"
            }))
        End Sub

        Private _Relations As New List(Of Lazy.Relation)
        Protected Overrides ReadOnly Property Relations As IEnumerable(Of Lazy.Relation)
            Get
                If _Relations.Count = 0 Then
                    CreateRelationHelper(RelationType.RELATION_ADD, Type.Float, Type.Float, $"{INSTANCE_ARGUMENT_NAME} + val")
                    CreateRelationHelper(RelationType.RELATION_ADD, Type.Int, Type.Float, $"{INSTANCE_ARGUMENT_NAME} + val")
                    CreateRelationHelper(RelationType.RELATION_SUB, Type.Float, Type.Float, $"{INSTANCE_ARGUMENT_NAME} - val")
                    CreateRelationHelper(RelationType.RELATION_SUB, Type.Int, Type.Float, $"{INSTANCE_ARGUMENT_NAME} - val")
                    CreateRelationHelper(RelationType.RELATION_MULT, Type.Float, Type.Float, $"{INSTANCE_ARGUMENT_NAME} * val")
                    CreateRelationHelper(RelationType.RELATION_MULT, Type.Int, Type.Float, $"{INSTANCE_ARGUMENT_NAME} * val")
                    CreateRelationHelper(RelationType.RELATION_DIV, Type.Float, Type.Float, $"(double)round({INSTANCE_ARGUMENT_NAME} / val)")
                    CreateRelationHelper(RelationType.RELATION_DIV, Type.Int, Type.Float, $"(double)round({INSTANCE_ARGUMENT_NAME} / val)")
                    CreateRelationHelper(RelationType.RELATION_MODULO, Type.Float, Type.Float, $"{INSTANCE_ARGUMENT_NAME} % val")

                    _Relations.Add(New Lazy.HardRelation(Me, RelationType.RELATION_UNARY_MINUS, {}, {}, Type.Float, {$"return -{INSTANCE_ARGUMENT_NAME};"}))
                End If
                Return _Relations
            End Get
        End Property

        Private Sub CreateRelationHelper(RelationType As RelationType, ValType As Type, ReturnType As Type, ReturnValue As String)
            _Relations.Add(New Lazy.HardRelation(Me, RelationType, {ValType}, {"val"}, ReturnType, {$"return {ReturnValue};"}))
        End Sub

    End Class
End Namespace