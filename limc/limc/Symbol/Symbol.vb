Namespace Symbol
    '
    ' Used in the HIR tree to represent a symbol (ex: a function, a variable)
    ' Define the existence in memory of the given ressource
    ' Map the "true" representation => AST Node
    ' Map the "c" representation => HIR Node
    '
    Friend MustInherit Class Symbol

        Public ReadOnly Property LimName As String

        Public Sub New(LimName As String)
            Me.LimName = LimName
        End Sub

    End Class
End Namespace