Imports limc.Symbol

Namespace HIR
    Friend MustInherit Class ExpressionNode
        Inherits Node

        Public ReadOnly Property ExpressionReturnType As TypeSymbol

        Protected Sub New(Location As Location, ExpressionReturnType As TypeSymbol)
            MyBase.New(Location)
            Me.ExpressionReturnType = ExpressionReturnType
        End Sub
        Public MustOverride Function Accept(Of R)(Visitor As IHIRExpressionVisitor(Of R)) As R

    End Class
End Namespace