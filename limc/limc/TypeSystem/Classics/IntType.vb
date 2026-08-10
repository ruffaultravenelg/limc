Namespace TypeSystem
    Public Class IntType
        Inherits Type

        Public Overrides ReadOnly Property cRepresentation As String = "int"
        Public Overrides ReadOnly Property IsPointer As Boolean = False

        Public Overrides Function DefaultValue(Scope As Context.Scope) As String
            Return "0"
        End Function

        Public Overrides Function ToString() As String
            Return "int"
        End Function

        Public Sub Compile()
            RegisterMethod(New Lazy.HardMethod(Me, "str", {}, Type.Str, {
                $"char* buffer = {LIM_ALLOC}({INT_TO_STR_BUFFERSIZE});",
                $"sprintf(buffer, ""%d"", {Constants.INSTANCE_ARGUMENT_NAME});",
                "return buffer;"
            }))
            RegisterMethod(New Lazy.HardMethod(Me, "float", {}, Type.Float, {
                $"return (double){Constants.INSTANCE_ARGUMENT_NAME};"
            }))
        End Sub

    End Class
End Namespace