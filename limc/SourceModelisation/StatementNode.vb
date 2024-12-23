Public MustInherit Class StatementNode
    Inherits Node

    'Constructor
    Public Sub New(Location As Location)
        MyBase.New(Location)
    End Sub

    'Compile
    Public MustOverride Sub Compile(Scope As Scope)

End Class
