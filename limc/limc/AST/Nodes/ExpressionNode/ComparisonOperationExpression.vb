Imports limc.TypeSystem.Type

Namespace AST
    Public Class ComparisonOperationExpression
        Inherits ExpressionNode

        Private Left As ExpressionNode
        Private Op As TypeSystem.RelationType
        Private Right As ExpressionNode

        Public Sub New(Left As ExpressionNode, Op As TypeSystem.RelationType, Right As ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.Left = Left
            Me.Op = Op
            Me.Right = Right
        End Sub

        Private Function GetRelation(LType As TypeSystem.Type, RType As TypeSystem.Type) As Lazy.Relation
            Return LType.GetRelation(Op, {RType}, Location)
        End Function

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type
            Dim leftType As TypeSystem.Type = Left.GetExpressionReturnType(Context)
            Dim rightType As TypeSystem.Type = Right.GetExpressionReturnType(Context)

            ' Try shortcut
            Dim fastType As TypeSystem.Type = TryHardcodedReturnType(leftType, rightType)
            If fastType IsNot Nothing Then
                Return fastType
            End If

            ' Fallback to relations
            Return GetRelation(leftType, rightType).ReturnType
        End Function

        Private Function TryHardcodedReturnType(L As TypeSystem.Type, R As TypeSystem.Type) As TypeSystem.Type

            ' INT|FLOAT [op] FLOAT|INT
            If (L Is Int OrElse L Is Float) AndAlso (R Is Int OrElse R Is Float) Then
                Select Case Op
                    Case TypeSystem.RelationType.RELATION_EQUAL,
                        TypeSystem.RelationType.RELATION_GREATERTHAN,
                        TypeSystem.RelationType.RELATION_GREATERTHANEQUAL,
                        TypeSystem.RelationType.RELATION_LESSTHAN,
                        TypeSystem.RelationType.RELATION_LESSTHANEQUAL
                        Return Bool
                    Case Else
                        Return Nothing
                End Select
            End If

            ' STR = STRING
            If L Is Str AndAlso R Is Str AndAlso Op = TypeSystem.RelationType.RELATION_EQUAL Then
                Return Bool
            End If

            Return Nothing
        End Function

        Public Overrides Function CompileExpression(Writer As CWriter, Scope As Context.Scope) As String
            Dim leftType As TypeSystem.Type = Left.GetExpressionReturnType(Scope)
            Dim rightType As TypeSystem.Type = Right.GetExpressionReturnType(Scope)

            ' Try shortcut
            Dim fastCompiled As String = TryHardcodedCompilation(leftType, rightType, Writer, Scope)
            If fastCompiled IsNot Nothing Then
                Return fastCompiled
            End If

            ' Fallback to relations
            Return GetRelation(leftType, rightType).CompileCall(Left, {Right}, Writer, Scope, Location)
        End Function

        Private Function TryHardcodedCompilation(L As TypeSystem.Type, R As TypeSystem.Type, Writer As CWriter, Scope As Context.Scope) As String

            ' INT|FLOAT [op] FLOAT|INT
            If (L Is Int OrElse L Is Float) AndAlso (R Is Int OrElse R Is Float) Then
                Dim lCode = Left.CompileExpression(Writer, Scope)
                Dim rCode = Right.CompileExpression(Writer, Scope)
                Select Case Op
                    Case TypeSystem.RelationType.RELATION_EQUAL
                        Return $"({lCode} == {rCode})"
                    Case TypeSystem.RelationType.RELATION_GREATERTHAN
                        Return $"({lCode} > {rCode})"
                    Case TypeSystem.RelationType.RELATION_GREATERTHANEQUAL
                        Return $"({lCode} >= {rCode})"
                    Case TypeSystem.RelationType.RELATION_LESSTHAN
                        Return $"({lCode} < {rCode})"
                    Case TypeSystem.RelationType.RELATION_LESSTHANEQUAL
                        Return $"({lCode} <= {rCode})"
                    Case Else
                        Return Nothing
                End Select
            End If

            ' STR = STRING
            If L Is Str AndAlso R Is Str AndAlso Op = TypeSystem.RelationType.RELATION_EQUAL Then
                Dim lCode = Left.CompileExpression(Writer, Scope)
                Dim rCode = Right.CompileExpression(Writer, Scope)
                Return $"(strcmp({lCode}, {rCode}) == 0)"
            End If

            Return Nothing
        End Function

    End Class
End Namespace