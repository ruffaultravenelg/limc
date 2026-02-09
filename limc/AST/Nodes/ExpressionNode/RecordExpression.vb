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
            If TypeOf RecordType IsNot TypeSystem.RecordType AndAlso TypeOf RecordType IsNot TypeSystem.StructType Then
                Throw New UnknownOrUnreachableElementError(Record.ToString(), Record.Location)
            End If
            Return RecordType
        End Function

        Public Overrides Function CompileExpression(Writer As CWriter, Scope As Context.Scope) As String
            Dim ConstructedType As TypeSystem.Type = Record.GetAssociatedType(Scope)

            If TypeOf ConstructedType Is TypeSystem.RecordType Then
                Return CompileRecord(ConstructedType, Writer, Scope)

            ElseIf TypeOf ConstructedType Is TypeSystem.StructType Then
                Return CompileStructure(ConstructedType, Writer, Scope)

            Else
                Throw New UnknownOrUnreachableElementError(Record.ToString(), Record.Location)
            End If

        End Function

        Public Function CompileRecord(RecordType As TypeSystem.RecordType, Writer As CWriter, Scope As Context.Scope) As String

            Dim Fields = RecordType.GetConstructionFields(Location)
            If Values.Count > Fields.Count Then
                Throw New SyntaxError($"This type can only take a maximum of {Fields.Count} values. {Values.Count} are given.", Location)
            End If
            If Values.Count < Fields.Count(Function(f) f.DefaultValue Is Nothing) Then
                Throw New SyntaxError($"At least {Fields.Count(Function(f) f.DefaultValue Is Nothing)} values were expected instead of {Values.Count}.", Location)
            End If

            Dim CompiledValues As New List(Of String)
            For i As Integer = 0 To Fields.Count - 1
                Dim Field = Fields(i)

                If i < Values.Count Then
                    Dim Value = Values(i)
                    If Value.GetExpressionReturnType(Scope) <> Field.Type Then
                        Throw New TypeMismatchError(Field.Type, Value.GetExpressionReturnType(Scope), Value.Location)
                    End If
                    CompiledValues.Add(Value.CompileExpression(Writer, Scope))
                Else
                    Dim TmpScope As New Context.Scope(RecordType.BoneContext, Location)
                    CompiledValues.Add(Field.DefaultValue.CompileExpression(Writer, TmpScope))
                End If

            Next

            Return "(" & RecordType.cRepresentation & "){" & String.Join(", ", CompiledValues) & "}"

        End Function

        Public Function CompileStructure(StructureType As TypeSystem.StructType, Writer As CWriter, Scope As Context.Scope) As String

            ' Compile types
            Dim ArgumentTypes = Values.Select(Function(arg) arg.GetExpressionReturnType(Scope))

            ' Get constructor
            Dim Constructor As Lazy.Constructor = StructureType.GetConstructor(ArgumentTypes)
            If Constructor Is Nothing Then
                Throw New UnknownOrUnreachableConstructorError(StructureType, ArgumentTypes, Location)
            End If

            ' Compile constructor call
            Return Constructor.CompileCall(Values, Writer, Scope, Location)

        End Function

    End Class
End Namespace