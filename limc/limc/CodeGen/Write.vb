Namespace CodeGen
    Public Module Write
        Public Function Allocate(Size As String) As String
            Return Constants.LIM_ALLOC & "(" & Size & ")"
        End Function
        Public Function Allocate(Sizeof As String, Count As String) As String
            Return Constants.LIM_ALLOC & "(sizeof(" & Sizeof & ") * " & Count & ")"
        End Function
        Public Function AllocateLeaf(Size As String) As String
            Return Constants.LIM_ALLOC_LEAF & "(" & Size & ")"
        End Function
        Public Function AllocateLeaf(Sizeof As String, Count As String) As String
            Return Constants.LIM_ALLOC_LEAF & "(sizeof(" & Sizeof & ") * " & Count & ")"
        End Function

        Public Function WritePanicCall(Value As String) As String
            Return $"{PANIC_FUNCTION_NAME}({RUNTIME_CONTEXT_VARIABLE_NAME}, {Value})"
        End Function

        Public Function Write_C_to_LimStr(Value As String) As String
            Return $"{C_STR_TO_LIM_STR}({Value})"
        End Function
        Public Function Write_LimStr_to_C(Value As String) As String
            Return $"{LIM_STR_TO_C_STR}({Value})"
        End Function

    End Module

End Namespace