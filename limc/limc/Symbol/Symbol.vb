Namespace Symbol
    '
    ' Used in the HIR tree to represent a symbol (ex: a function, a variable)
    ' Define the existence in memory of the given ressource. It can therefore be used as a pointer to replace string names
    '
    ' it contains the signature of the element
    '   ex: for a function it's name, arguments & return type
    '
    ' it may also contains compilation data for constructs
    '   ex: AST Node, double dispatch with IHIRLowerer
    '
    Friend MustInherit Class Symbol

        Public ReadOnly Property LimName As String

        Public Sub New(LimName As String)
            Me.LimName = LimName
        End Sub

    End Class
End Namespace