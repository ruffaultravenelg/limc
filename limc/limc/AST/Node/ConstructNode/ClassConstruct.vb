Namespace AST
    Friend Class ClassConstruct
        Inherits ConstructNode

        Public ReadOnly Property ClassName As String
        Public ReadOnly Property GenericTypes As IReadOnlyList(Of GenericTypeParameterNode)

        Public ReadOnly Property Properties As IReadOnlyList(Of PropertieNode)
        Public ReadOnly Property Constructors As IReadOnlyList(Of ConstructorConstruct)

        Public Sub New(ClassName As String, GenericTypes As IEnumerable(Of GenericTypeParameterNode), Properties As IEnumerable(Of PropertieNode), Constructors As IEnumerable(Of ConstructorConstruct), Location As Location)
            MyBase.New(Location)
            Me.ClassName = ClassName
            Me.GenericTypes = GenericTypes.ToList()
            Me.Properties = Properties.ToList()
            Me.Constructors = Constructors.ToList()
        End Sub

        Public Overrides Function Accept(Of D, R)(Visitor As IConstructVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitClassConstruct(Me, Data)
        End Function

    End Class
End Namespace