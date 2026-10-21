Namespace AST
    Friend Class FunctionConstruct
        Inherits ConstructNode

        Public ReadOnly Property Name As String
        Public ReadOnly Property Arguments As IReadOnlyList(Of ArgumentNode)
        Public ReadOnly Property GenericTypes As IReadOnlyList(Of GenericTypeParameterNode)
        Public ReadOnly Property ReturnType As GenericTypeNode ' Nullable
        Public ReadOnly Property Body As BlockStatement
        Public Sub New(Name As String, GenericTypes As IEnumerable(Of GenericTypeParameterNode), Arguments As IEnumerable(Of ArgumentNode), ReturnType As GenericTypeNode, Body As BlockStatement, Location As Location)
            MyBase.New(Location)
            Me.Name = Name
            Me.GenericTypes = GenericTypes
            Me.Arguments = Arguments
            Me.ReturnType = ReturnType
            Me.Body = Body
        End Sub

        Public Overloads Overrides Function Accept(Of D, R)(Visitor As IConstructVisitor(Of D, R), Data As D) As R
            Return Visitor.VisitFunctionConstruct(Me, Data)
        End Function

    End Class

End Namespace