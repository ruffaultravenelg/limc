Namespace AST
    Public MustInherit Class ConstructNode
        Inherits Node

        Private _Exported As Boolean = False
        Public ReadOnly Property Exported As Boolean
            Get
                Return _Exported
            End Get
        End Property

        Public Sub SetExported(NewValue As Boolean)
            _Exported = NewValue
        End Sub

        Public Sub New(Location As Location)
            MyBase.New(Location)
        End Sub

    End Class

End Namespace