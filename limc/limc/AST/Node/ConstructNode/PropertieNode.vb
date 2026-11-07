Namespace AST
    Friend Class PropertieNode
        Inherits ConstructNode

        Public ReadOnly Property PropertieName As String
        Public ReadOnly Property PropertieDefaultValue As ExpressionNode

        Public Sub New(PropertieName As String, PropertieDefaultValue As ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.PropertieName = PropertieName
            Me.PropertieDefaultValue = PropertieDefaultValue
        End Sub

        Public Overrides Function Accept(Of D, R)(Visitor As IConstructVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitPropertieNode(Me, Data)
        End Function

    End Class
End Namespace