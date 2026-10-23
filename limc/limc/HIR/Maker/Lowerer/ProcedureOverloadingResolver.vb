Imports limc.Symbol

Namespace HIR.Lowerer
    Friend Class ProcedureOverloadingResolver

        ' Return the FIRST procedure that match each argument types
        ' if no match, returns Nothing
        ' => assumes name & generics are already checked -> only check matching argument types
        Public Shared Function SelectBestMatch(Matches As IEnumerable(Of SymbolMatch), HintedTypes As IReadOnlyList(Of TypeSymbol)) As SymbolMatch
            For Each Match In Matches
                Select Case True

                    Case Match.IsType(Of FunctionSymbol)
                        If IsFunctionSymbolMatching(Match.FUNCTION_SYMBOL, HintedTypes) Then
                            Return Match
                        End If

                    Case Match.IsType(Of VariableSymbol)
                        If IsVariableSymbolMatching(Match.VARIABLE_SYMBOL, HintedTypes) Then
                            Return Match
                        End If

                End Select
            Next
            Return Nothing 'No match found
        End Function

        Public Shared Function IsFunctionSymbolMatching(Func As FunctionSymbol, HintedTypes As IReadOnlyList(Of TypeSymbol)) As Boolean
            Return Func.Arguments.Select(Function(arg) arg.VariableType).SequenceEqual(HintedTypes) ' Compare list of func.args.type to hintedtypes
        End Function
        Public Shared Function IsVariableSymbolMatching(Var As VariableSymbol, HintedTypes As IReadOnlyList(Of TypeSymbol)) As Boolean
            If TypeOf Var.VariableType IsNot FuncTypeSymbol Then
                Return False
            End If
            Return DirectCast(Var.VariableType, FuncTypeSymbol).ProcArgumentTypes.SequenceEqual(HintedTypes) ' Compare list of vartype(functype).argtypes to hintedtypes
        End Function

    End Class
End Namespace