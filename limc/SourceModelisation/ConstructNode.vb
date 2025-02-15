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

    'Export all constructs if none of them are exported
    Public Shared Sub HandleExports(Constructs As IEnumerable(Of ConstructNode))

        'If at least one constuct is exported then exit the functions
        For Each Construct As ConstructNode In Constructs
            If Construct.Exported Then
                Return
            End If
        Next

        'Export all constructs
        For Each Construct As ConstructNode In Constructs
            Construct.SetExported(True)
        Next

    End Sub

End Class
