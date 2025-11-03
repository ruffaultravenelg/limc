Namespace AST
    Public Class ArrayTypeNode
        Inherits AST.TypeNode

        Private ReadOnly Count As Integer
        Private ReadOnly Type As AST.TypeNode

        Public Sub New(Count As Integer, Type As AST.TypeNode, Location As Location)
            MyBase.New(Location)
            Me.Count = Count
            Me.Type = Type
        End Sub

        Public Overrides Function GetAssociatedType(Context As Context.Context) As TypeSystem.Type
            Throw New NotImplementedException()
        End Function

    End Class

End Namespace