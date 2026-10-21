Imports limc.Symbol

Namespace Environment
    Friend Class GlobalEnvironment
        Inherits Env

        Public Sub New()
            MyBase.New(Nothing)
        End Sub

        Protected Overrides Iterator Function ResolveSymbol(ElementName As String, PassedGenericTypes As IReadOnlyList(Of TypeSymbol)) As IEnumerable(Of SymbolMatch)
            ' No global symbols
        End Function

        Protected Overrides Function ResolveType(Typename As String, PassedGenericTypes As IReadOnlyList(Of TypeSymbol)) As TypeSymbol
            If PassedGenericTypes.Count = 0 Then
                Select Case Typename

                    Case "int"
                        Return TypeSymbol.INT

                End Select
            End If
            Return Nothing
        End Function

    End Class
End Namespace