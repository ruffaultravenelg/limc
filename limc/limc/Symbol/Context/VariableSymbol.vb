Namespace Symbol
    '
    ' Used in the HIR tree to represent a symbol (ex: a function, a variable)
    ' Define the existence in memory of the given ressource
    ' Map the "true" representation => AST Node
    ' Map the "c" representation => HIR Node
    '
    Friend Class VariableSymbol
        Inherits Symbol

        Public ReadOnly Property VariableType As TypeSymbol

        Public Sub New(VariableName As String, VariableType As TypeSymbol)
            MyBase.New(VariableName)
            Me.VariableType = VariableType
        End Sub

    End Class
End Namespace