Imports limc.Symbol

Namespace Environment
    '
    ' Used for Statement & Expression compilation => allow for anyone to add variables
    '
    Friend Class BlockEnv
        Inherits Env

        Private VariableStore As New Dictionary(Of String, VariableSymbol)

        Public Sub New(Parent As Env)
            MyBase.New(Parent)
        End Sub

        Public Function DefineVariable(VariableName As String, VariableType As TypeSymbol) As VariableSymbol
            Dim var = New VariableSymbol(VariableName, VariableType)
            VariableStore(VariableName) = var
            Return var
        End Function
        Public Function DefineVariable(VarSymbol As VariableSymbol) As VariableSymbol
            VariableStore(VarSymbol.LimName) = VarSymbol
            Return VarSymbol
        End Function

        Protected Overrides Iterator Function ResolveSymbol(ElementName As String, PassedGenericTypes As IReadOnlyList(Of Symbol.TypeSymbol)) As IEnumerable(Of SymbolMatch)
            If PassedGenericTypes.Count = 0 AndAlso VariableStore.ContainsKey(ElementName) Then
                Yield New SymbolMatch(VariableStore(ElementName))
            End If
        End Function

        Protected Overrides Function ResolveType(Typename As String, PassedGenericTypes As IReadOnlyList(Of TypeSymbol)) As TypeSymbol
            Return Nothing
        End Function

    End Class
End Namespace