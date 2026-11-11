Namespace HIR
    Friend MustInherit Class ConstructNode
        Inherits Node

        Protected Sub New(Location As Location)
            MyBase.New(Location)
        End Sub
        Public MustOverride Sub Accept(Visitor As IHIRVisitor)

    End Class
End Namespace