Imports System.Text.RegularExpressions

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

                    ' GET [idx]
                    _Relations.Add(New Lazy.HardRelation(Me, RelationType.RELATION_BRACKETS, {Type.Int}, {"index"}, ElementType, {
                        $"if (index < 0) index = {Length} + index;",
                        $"if (index >= {Length} || index < 0) {CodeGen.WritePanicCall("""Index out of range""")};",
                        $"return {INSTANCE_ARGUMENT_NAME}[index];"
                    }))

                    ' SET [idx]
                    '_Relations.Add(New Lazy.HardRelation(Me, RelationType.RELATION_SET_BRACKETS, {Type.Int, ElementType}, {"index", "newValue"}, Nothing, {
                    '    $"if (index < 0) index = {Length} + index;",
                    '    $"if (index >= {Length} || index < 0) {CodeGen.WritePanicCall("""Index out of range""")};",
                    '    $"{INSTANCE_ARGUMENT_NAME}[index] = newValue;"
                    '}))

                End If
                Return _Relations
            End Get
        End Property

        Public Sub WriteElementAssignation(Instance As AST.ExpressionNode, Index As AST.ExpressionNode, NewValue As AST.ExpressionNode, Writer As CWriter, Scope As Context.Scope)

            'Check index type
            If Index.GetExpressionReturnType(Scope) <> Type.Int Then
                Throw New TypeMismatchError(Type.Int, Index.GetExpressionReturnType(Scope), Index.Location)
            End If

            ' Check new value type
            If NewValue.GetExpressionReturnType(Scope) <> ElementType Then
                Throw New TypeMismatchError(ElementType, NewValue.GetExpressionReturnType(Scope), NewValue.Location)
            End If

            ' Compile assignation
            Writer.WriteLine($"{Instance.CompileExpression(Writer, Scope)}[{Index.CompileExpression(Writer, Scope)}] = {NewValue.CompileExpression(Writer, Scope)};")
            'TODO: no index check??????? just use RELATION_SET_BRACKETS relation

        End Sub

        ' Structs bodies are defined before Racks typedef, use C definition (type var[n]) instead of typedef (type var)
        Public Shared Function ValidateStructFieldDefinition(Field As String) As String
            Dim FieldType As String = Nothing
            Dim FieldName As String = Nothing

            If CheckAndExtract(Field, FieldType, FieldName) Then
                Dim Dimensions As New List(Of Integer)
                Dim CurrentType As TypeSystem.RackType = GetRackTypeFromCRepresentation(FieldType)
                Dim BaseType As TypeSystem.Type = CurrentType.ElementType
                Dimensions.Add(CurrentType.Length)
                While TypeOf BaseType Is RackType
                    Dimensions.Add(DirectCast(BaseType, RackType).Length)
                    CurrentType = BaseType
                    BaseType = CurrentType.ElementType
                End While
                Return BaseType.cRepresentation & " " & FieldName & String.Concat(Dimensions.Select(Function(d) $"[{d}]")) & ";"
            Else
                Return Field
            End If
        End Function
        Private Shared Function GetRackTypeFromCRepresentation(Representation As String) As RackType
            For Each D In Racks.Values
                For Each T In D.Values
                    If T.cRepresentation = Representation Then
                        Return T
                    End If
                Next
            Next
            Throw New InternalError()
        End Function
        Private Shared Function CheckAndExtract(ByVal Field As String, ByRef Type As String, ByRef Name As String) As Boolean
            Dim pattern As String = "^(?<X>\w+)_r (?<Y>\w+);$"

            Dim match As Match = Regex.Match(Field, pattern)

            If match.Success Then
                Type = match.Groups("X").Value & "_r"
                Name = match.Groups("Y").Value
                Return True
            Else
                Type = Nothing
                Name = Nothing
                Return False
            End If
        End Function


    End Class
End Namespace