Imports limc.Symbol

Namespace Repository
    '
    ' Used in Primitives, not in environments
    '
    Friend Class FuncTypeRepository

        ' func<a, b><r>
        Private Shared ReadOnly FuncTypes As New Dictionary(Of (Arguments As IEnumerable(Of TypeSymbol), Ret As TypeSymbol), FuncTypeSymbol)(New Comparer.FuncTypeComparer)

        Public Function ResolveFuncType(ArgumentTypes As IEnumerable(Of TypeSymbol), ReturnType As TypeSymbol) As FuncTypeSymbol
            Dim Key = (ArgumentTypes, ReturnType)

            If FuncTypes.ContainsKey(Key) Then
                Return FuncTypes(Key)
            Else
                Dim newSymbol As New FuncTypeSymbol(ArgumentTypes, ReturnType)
                FuncTypes(Key) = newSymbol
                Return newSymbol
            End If

        End Function

    End Class
End Namespace