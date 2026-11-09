Imports limc.Symbol

Namespace Environment
    Friend Class GlobalEnvironment
        Inherits Env

        Private Primitives As IPrimitives

        Public Sub New(Primitives As IPrimitives)
            MyBase.New(Nothing)
            Me.Primitives = Primitives
        End Sub

        Protected Overrides Iterator Function ResolveSymbol(ElementName As String, PassedGenericTypes As IReadOnlyList(Of TypeSymbol)) As IEnumerable(Of SymbolMatch)
            If PassedGenericTypes.Count = 0 Then
                Select Case ElementName
                    Case "puts"
                        Yield New SymbolMatch(Primitives.PUTS)
                End Select
            End If
        End Function

        Protected Overrides Function ResolveType(Typename As String, PassedGenericTypes As IReadOnlyList(Of TypeSymbol)) As TypeSymbol
            If PassedGenericTypes.Count = 0 Then
                Select Case Typename

                    Case "int"
                        Return Primitives.INT
                    Case "float"
                        Return Primitives.FLOAT
                    Case "bool"
                        Return Primitives.BOOL
                    Case "str"
                        Return Primitives.STR

                End Select
            End If
            Return Nothing
        End Function

    End Class
End Namespace