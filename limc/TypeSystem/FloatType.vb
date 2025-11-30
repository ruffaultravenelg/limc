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

        Protected Overrides ReadOnly Property Relations As IEnumerable(Of Lazy.Relation) = {} 'Operations are hardcoded in nodes

    End Class
End Namespace