Namespace TypeSystem
    Public Class IntType
        Inherits Type

        Public Overrides ReadOnly Property cRepresentation As String = "int"

        Public Overrides Function DefaultValue(Scope As Context.Scope) As String
            Return "0"
        End Function

        Public Overrides Function ToString() As String
            Return "int"
        End Function

        Public Overrides Function RetrieveElements(Name As String) As IEnumerable(Of SearchMatch)
            Select Case Name
                Case "str"
                    Return {New SearchMatch(str_method)}
                Case Else
                    Return {}
            End Select
        End Function

        Private _str_method As Lazy.HardMethod = Nothing
        Private ReadOnly Property str_method As Lazy.HardMethod
            Get
                If _str_method Is Nothing Then
                    _str_method = New Lazy.HardMethod(
                       Me,
                        "str",
                        {},
                        Type.Str,
                        {
                            $"char* buffer = {LIM_ALLOC}({INT_TO_STR_BUFFERSIZE});",
                            $"sprintf(buffer, ""%d"", {Constants.INSTANCE_ARGUMENT_NAME});",
                            "return buffer;"
                        }
                    )
                End If
                Return _str_method
            End Get
        End Property

        Private _Relations As New List(Of Lazy.Relation)
        Protected Overrides ReadOnly Property Relations As IEnumerable(Of Lazy.Relation)
            Get
                If _Relations.Count = 0 Then
                    CreateRelationHelper(RelationType.RELATION_ADD, Type.Int, Type.Int, $"{INSTANCE_ARGUMENT_NAME} + val")
                    CreateRelationHelper(RelationType.RELATION_ADD, Type.Float, Type.Int, $"{INSTANCE_ARGUMENT_NAME} + val")
                    CreateRelationHelper(RelationType.RELATION_SUB, Type.Int, Type.Int, $"{INSTANCE_ARGUMENT_NAME} - val")
                    CreateRelationHelper(RelationType.RELATION_SUB, Type.Float, Type.Int, $"{INSTANCE_ARGUMENT_NAME} - val")
                    CreateRelationHelper(RelationType.RELATION_MULT, Type.Int, Type.Int, $"{INSTANCE_ARGUMENT_NAME} * val")
                    CreateRelationHelper(RelationType.RELATION_MULT, Type.Float, Type.Int, $"{INSTANCE_ARGUMENT_NAME} * val")
                    CreateRelationHelper(RelationType.RELATION_DIV, Type.Int, Type.Int, $"(int)round({INSTANCE_ARGUMENT_NAME} / val)")
                    CreateRelationHelper(RelationType.RELATION_DIV, Type.Float, Type.Int, $"(int)round({INSTANCE_ARGUMENT_NAME} / val)")
                    CreateRelationHelper(RelationType.RELATION_MODULO, Type.Int, Type.Int, $"{INSTANCE_ARGUMENT_NAME} % val")

                    _Relations.Add(New Lazy.HardRelation(Me, RelationType.RELATION_UNARY_MINUS, {}, {}, Type.Int, {$"return -{INSTANCE_ARGUMENT_NAME};"}))
                End If
                Return _Relations
            End Get
        End Property

        Private Sub CreateRelationHelper(RelationType As RelationType, ValType As Type, ReturnType As Type, ReturnValue As String)
            _Relations.Add(New Lazy.HardRelation(
                Me,
                RelationType,
                {ValType},
                {"val"},
                ReturnType,
                {$"return {ReturnValue};"}
             ))
        End Sub

    End Class
End Namespace