Namespace AST
    Public Class ArgumentNode
        Inherits Node

        Public ReadOnly Property ArgumentName As String
        Public ReadOnly Property ArgumentType As AST.TypeNode

        Public Sub New(ArgumentName As String, ArgumentType As AST.TypeNode, Location As Location)
            MyBase.New(Location)
            Me.ArgumentName = ArgumentName
            Me.ArgumentType = ArgumentType
        End Sub

    End Class
End Namespace