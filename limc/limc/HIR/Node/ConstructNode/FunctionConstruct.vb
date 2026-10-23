Imports limc.Symbol

Namespace HIR
    '
    ' A function with its generic types
    ' A AST.FunctionConstruct could create mutliple HIR.FunctionConstruct
    '
    Friend Class FunctionConstruct
        Inherits ConstructNode

        Public ReadOnly Property Symbol As Symbol.FunctionSymbol
        Public ReadOnly Property Body As BlockStatement

        Public Sub New(Location As Location, Symbol As FunctionSymbol, Body As BlockStatement)
            MyBase.New(Location)
            Me.Symbol = Symbol
            Me.Body = Body
        End Sub
        Public Overrides Sub Accept(Visitor As IHIRVisitor)
            Visitor.VisitFunctionConstruct(Me)
        End Sub

    End Class
End Namespace