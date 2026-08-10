Namespace AST
    Public MustInherit Class Node

        Public ReadOnly Property Location As Location
        Public Sub New(Location As Location)
            Me.Location = Location
        End Sub

    End Class

End Namespace