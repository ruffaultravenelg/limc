Public Module FunctionPointers

    '=====================================================
    '======== ALL COMPILED FUNCTION POINTER TYPES ========
    '=====================================================
    Private AllCompiledFunctionPointerTypes As New List(Of FunctionSignatureType)

    '============================
    '======== FROM TYPES ========
    '============================
    Public ReadOnly Property FromTypes(FunctionArgumentsTypes As IEnumerable(Of Type), Optional FunctionReturnType As Type = Nothing) As FunctionSignatureType
        Get

            'If type already compiled
            For Each CompiledfunctionPointerType As FunctionSignatureType In AllCompiledFunctionPointerTypes
                If CompiledfunctionPointerType.LooksLike(FunctionArgumentsTypes, FunctionReturnType) Then
                    Return CompiledfunctionPointerType
                End If
            Next

            'Create type
            Dim FunctionPointerType As New FunctionSignatureType(FunctionArgumentsTypes, FunctionReturnType)
            AllCompiledFunctionPointerTypes.Add(FunctionPointerType)
            Return FunctionPointerType

        End Get
    End Property

End Module
