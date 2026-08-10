Namespace TypeSystem
    Public Class FloatType
        Inherits Type

        Public Overrides ReadOnly Property cRepresentation As String = "double"
        Public Overrides ReadOnly Property IsPointer As Boolean = False

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
            RegisterMethod(New Lazy.HardMethod(Me, "round", {}, Type.Int, {
                $"return (int)round({INSTANCE_ARGUMENT_NAME});"
            }))
            RegisterMethod(New Lazy.HardMethod(Me, "floor", {}, Type.Int, {
                $"return (int)floor({INSTANCE_ARGUMENT_NAME});"
            }))
            RegisterMethod(New Lazy.HardMethod(Me, "ceil", {}, Type.Int, {
                $"return (int)ceil({INSTANCE_ARGUMENT_NAME});"
            }))
        End Sub

    End Class
End Namespace