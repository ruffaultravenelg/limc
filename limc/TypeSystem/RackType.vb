Namespace TypeSystem
    Public Class RackType
        Inherits Type

        Private Length As Integer
        Private ElementType As Type
        Private TypedefName As String

        Private Sub New(Length As Integer, ElementType As Type)
            Me.Length = Length
            Me.ElementType = ElementType
            TypedefName = CodeGen.Namer.Rack(ToString())
            CodeGen.RegisterTypedef($"typedef {ElementType.cRepresentation} {TypedefName}[{Length}];")
        End Sub

        Public Overrides ReadOnly Property cRepresentation As String
            Get
                Return TypedefName
            End Get
        End Property

        Public Overrides Function DefaultValue(Scope As Context.Scope) As String
            Dim DefaultValues As New List(Of String)
            For i As Integer = 0 To Length - 1
                DefaultValues.Add(ElementType.DefaultValue(Scope))
            Next
            Return "{" & String.Join(", ", DefaultValues) & "}"
        End Function

        Public Overrides Function RetrieveElements(Name As String) As IEnumerable(Of SearchMatch)
            Return {}
        End Function

        Public Overrides Function ToString() As String
            Return $"{Length}<{ElementType}>"
        End Function

        ' Extern access
        Private Shared Racks As New Dictionary(Of Integer, Dictionary(Of Type, RackType))

        Public Shared Function FromLengthAndType(Length As Integer, ElementType As Type) As RackType

            ' Get dictionnary from it's length
            If Not Racks.ContainsKey(Length) Then
                Racks(Length) = New Dictionary(Of Type, RackType)
            End If
            Dim TypesOfThisLength As Dictionary(Of Type, RackType) = Racks(Length)

            ' Get type
            If Not TypesOfThisLength.ContainsKey(ElementType) Then
                TypesOfThisLength(ElementType) = New RackType(Length, ElementType)
            End If
            Return TypesOfThisLength(ElementType)

        End Function

        Private _Relations As New List(Of Lazy.Relation)
        Protected Overrides ReadOnly Property Relations As IEnumerable(Of Lazy.Relation)
            Get
                If _Relations.Count = 0 Then

                    _Relations.Add(New Lazy.HardRelation(Me, RelationType.RELATION_BRACKETS, {Type.Int}, {"index"}, ElementType, {
                        $"if (index < 0) index = {Length} + index;",
                        $"if (index >= {Length} || index < 0) {CodeGen.WritePanicCall("""Index out of range""")};",
                        $"return {INSTANCE_ARGUMENT_NAME}[index];"
                    }))

                End If
                Return _Relations
            End Get
        End Property

        Public Sub WriteElementAssignation(Instance As AST.ExpressionNode, Index As AST.ExpressionNode, NewValue As AST.ExpressionNode, Scope As Context.Scope)

            'Check index type
            If Index.GetExpressionReturnType(Scope) <> Type.Int Then
                Throw New TypeMismatchError(Type.Int, Index.GetExpressionReturnType(Scope), Index.Location)
            End If

            ' Check new value type
            If NewValue.GetExpressionReturnType(Scope) <> ElementType Then
                Throw New TypeMismatchError(ElementType, NewValue.GetExpressionReturnType(Scope), NewValue.Location)
            End If

            ' Compile assignation
            Scope.WriteLine($"{Instance.CompileExpression(Scope)}[{Index.CompileExpression(Scope)}] = {NewValue.CompileExpression(Scope)};")

        End Sub

    End Class
End Namespace