Namespace TypeSystem

    Public MustInherit Class Type

        Public MustOverride ReadOnly Property cRepresentation As String
        Public MustOverride Function DefaultValue(Scope As Scope) As String

    End Class

End Namespace