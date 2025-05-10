Public MustInherit Class StatementNode
    Inherits Node

    'Constructor
    Public Sub New(Location As Location)
        MyBase.New(Location)
    End Sub

    'Compile
    Public MustOverride Sub Compile(Scope As Scope)

    'Contains return statement
    Public MustOverride ReadOnly Property ContainsReturnStatement As Boolean

    'Contains return
    Public Shared Function ListContainsReturnStatement(Statements As IEnumerable(Of StatementNode)) As Boolean
        For Each Statement As StatementNode In Statements
            If Statement.ContainsReturnStatement Then
                Return True
            End If
        Next
        Return False
    End Function

End Class
