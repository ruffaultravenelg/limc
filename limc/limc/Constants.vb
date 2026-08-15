Public Module Constants

    'WARNING: All the following constants are accessible via a source statement (@CONSTANT_NAME)
    Public Const RUNTIME_CONTEXT_STRUCT_NAME As String = "ctx_t"
    Public Const RUNTIME_CONTEXT_VARIABLE_NAME As String = "c"
    Public Const RUNTIME_PARENT_CONTEXT_VARIABLE_NAME As String = "parent_ctx"
    Public Const PANIC_FUNCTION_NAME As String = "lim_panic"
    Public Const PRINT_STACK_TRACE_FUNCTION_NAME As String = "lim_printStackTrace"
    Public Const LIM_ALLOC As String = "LIM_ALLOC"
    Public Const LIM_ALLOC_LEAF As String = "LIM_ALLOC_LEAF"
    Public Const LIM_FREE As String = "LIM_FREE"
    Public Const LIM_REALLOC As String = "LIM_REALLOC"
    Public Const INT_TO_STR_BUFFERSIZE As String = "12"
    Public Const FLOAT_TO_STR_BUFFERSIZE As String = "12"
    Public Const GETS_BUFFER_SIZE As String = "100"
    Public Const INSTANCE_ARGUMENT_NAME As String = "instance"
    Public Const SELF As String = INSTANCE_ARGUMENT_NAME
    Public Const LIM_STR As String = "lim_str"
    Public Const LIM_STR_TO_C_STR As String = "lstrtoc"
    Public Const C_STR_TO_LIM_STR As String = "ctolstr"

End Module