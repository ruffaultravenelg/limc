Namespace AST
    Friend MustInherit Class ConstructNode
        Inherits Node

        Public Sub New(Location As Location)
            MyBase.New(Location)
        End Sub

        Public MustOverride Overloads Function Accept(Of D, R)(Visitor As IConstructVisitor(Of D, R), Data As D) As R
        Public NotOverridable Overloads Overrides Function Accept(Of D, R)(Visitor As IASTVisitor(Of D, R), Data As D) As R
            Return Accept(DirectCast(Visitor, IConstructVisitor(Of D, R)), Data)
        End Function

    End Class
End Namespace