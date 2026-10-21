Imports limc.Symbol

'
' Environement symbol search result
' Used to allow for simple if cases and cast
'
Friend Class SymbolMatch

    Private _Value As Symbol.Symbol

    ' ==== COMPARISON ====
    Public Function IsType(Of T As Symbol.Symbol)() As Boolean
        Return TypeOf _Value Is T
    End Function
    Public Function CastAs(Of T As Symbol.Symbol)() As T
        Return _Value
    End Function

    ' ==== FUNCTION SYMBOL ====
    Public Sub New(Val As FunctionSymbol)
        _Value = Val
    End Sub
    Public ReadOnly Property FUNCTION_SYMBOL As FunctionSymbol
        Get
            Return _Value
        End Get
    End Property

    ' ==== VARIABLE SYMBOL ====
    Public Sub New(Val As VariableSymbol)
        _Value = Val
    End Sub
    Public ReadOnly Property VARIABLE_SYMBOL As VariableSymbol
        Get
            Return _Value
        End Get
    End Property

End Class
