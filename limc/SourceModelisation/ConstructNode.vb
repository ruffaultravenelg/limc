Public MustInherit Class ConstructNode
    Inherits Node

    'Exported -> Propertie is readonly to avoid changing export state being "normal"
    Public ReadOnly Property Exported As Boolean
        Get
            Return _Exported
        End Get
    End Property
    Private _Exported As Boolean = False
    Public Sub SetExported(Value As Boolean) 'Only use after parsing a all FileAST
        _Exported = Value
    End Sub

    Public Sub New(Location As Location)
        MyBase.New(Location)
    End Sub

End Class
