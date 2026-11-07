Imports limc.HIR

Namespace C
    Friend Class ConstructResolver

        ' Takes a HIR.ProgramNode and returns each nodes that transform into a C function (used to print all func signatures)
        Public Shared Iterator Function FindAllFunctionSignatures(Program As ProgramNode) As IEnumerable(Of FunctionSignatureInfo)

            ' Functions
            For Each Fun In Program.Functions
                Yield New FunctionSignatureInfo(Fun.Symbol, Fun.Symbol.ReturnType, Fun.Symbol.Arguments)
            Next

            ' others... (constructors, methods, etc)

        End Function

    End Class
End Namespace