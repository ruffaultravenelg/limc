Namespace HIR

    ' HIR: High-level Intermediate Representation
    ' ~ AST with resolved symbols & generics

    Friend MustInherit Class Node
        Public ReadOnly Property Location As Location

        Public Sub New(Location As Location)
            Me.Location = Location
        End Sub
        Public MustOverride Sub Accept(Visitor As IHIRVisitor)

    End Class

End Namespace