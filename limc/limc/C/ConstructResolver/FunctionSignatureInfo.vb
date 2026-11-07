Imports limc.Symbol

'
' Represent a symbol that need to be transformed into a C function
'
Friend Class FunctionSignatureInfo

    Public ReadOnly Property Symbol As Symbol.Symbol
    Public ReadOnly Property ReturnType As TypeSymbol
    Public ReadOnly Property Arguments As IReadOnlyList(Of VariableSymbol)

    Public Sub New(Symbol As Symbol.Symbol, ReturnType As TypeSymbol, Arguments As IReadOnlyList(Of VariableSymbol))
        Me.Symbol = Symbol
        Me.ReturnType = ReturnType
        Me.Arguments = Arguments
    End Sub

End Class
