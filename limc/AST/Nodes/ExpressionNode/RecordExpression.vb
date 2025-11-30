Namespace AST
    Public Class RecordExpression
        Inherits ExpressionNode
        Implements IConstantExpression

        Private Record As TypeNode
        Private Values As IEnumerable(Of ExpressionNode)

        Public Sub New(Record As TypeNode, Values As IEnumerable(Of ExpressionNode), Location As Location)
            MyBase.New(Location)
            Me.Record = Record
            Me.Values = Values
        End Sub

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type
            Dim RecordType As TypeSystem.Type = Record.GetAssociatedType(Context)
            If TypeOf RecordType IsNot TypeSystem.RecordType Then
                Throw New UnknownOrUnreachableElementError(Record.ToString(), Record.Location)
            End If
            Return RecordType
        End Function

        Public Overrides Function CompileExpression(Scope As Context.Scope) As String
            Dim RecordType As TypeSystem.Type = Record.GetAssociatedType(Scope)
            If TypeOf RecordType IsNot TypeSystem.RecordType Then
                Throw New UnknownOrUnreachableElementError(Record.ToString(), Record.Location)
            End If

            Dim Fields = DirectCast(RecordType, TypeSystem.RecordType).GetConstructionFields(Location)
            If Fields.Count <> Values.Count Then
                Throw New SyntaxError($"{Fields.Count} values were expected instead of {Values.Count}.", Location)
            End If

            Dim CompiledValues As New List(Of String)
            For i As Integer = 0 To Fields.Count - 1
                Dim Field = Fields(i)
                Dim Value = Values(i)

                If Value.GetExpressionReturnType(Scope) <> Field.Type Then
                    Throw New TypeMismatchError(Field.Type, Value.GetExpressionReturnType(Scope), Value.Location)
                End If
                CompiledValues.Add(Value.CompileExpression(Scope))
            Next

            Return "(" & RecordType.cRepresentation & "){" & String.Join(", ", CompiledValues) & "}"

        End Function

    End Class
End Namespace