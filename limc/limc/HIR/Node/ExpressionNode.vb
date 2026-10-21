Imports limc.Symbol

Namespace HIR
    Friend MustInherit Class ExpressionNode
        Inherits Node

        Public ReadOnly Property ExpressionReturnType As TypeSymbol

        Protected Sub New(Location As Location, ExpressionReturnType As TypeSymbol)
            MyBase.New(Location)
            Me.ExpressionReturnType = ExpressionReturnType
        End Sub

    End Class
End Namespace