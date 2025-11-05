Namespace AST
    Public Class RackTypeNode
        Inherits AST.TypeNode

        Private ReadOnly Count As Integer
        Private ReadOnly Type As AST.TypeNode

        Public Sub New(Count As Integer, Type As AST.TypeNode, Location As Location)
            MyBase.New(Location)
            Me.Count = Count
            Me.Type = Type

            If Count < 1 Then
                Throw New SyntaxError("Cannot create a null or negative array of elements", Location)
            End If
        End Sub

        Public Overrides Function GetAssociatedType(Context As Context.Context) As TypeSystem.Type
            Return TypeSystem.RackType.FromLengthAndType(Count, Type.GetAssociatedType(Context))
        End Function

    End Class

End Namespace