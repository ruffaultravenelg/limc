Imports limc.Symbol

Namespace HIR
    Friend Class ArgumentNode
        Inherits ConstructNode

        Public ReadOnly Property Symbol As Symbol.VariableSymbol

        Public Sub New(Location As Location, Symbol As Symbol.Symbol)
            MyBase.New(Location)
            Me.Symbol = Symbol
        End Sub

        Public Overrides Sub Accept(Visitor As IHIRVisitor)
            Visitor.VisitArgumentNode(Me)
        End Sub

    End Class
End Namespace