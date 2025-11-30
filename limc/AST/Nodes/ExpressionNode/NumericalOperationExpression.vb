Imports limc.TypeSystem.Type

Namespace AST
    Public Class NumericalOperationExpression
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

            ' INT [op] INT
            If L Is Int AndAlso R Is Int Then
                If Op = TypeSystem.RelationType.RELATION_DIV Then
                    Return Float 'Division alwas returns a FLOAT
                Else
                    Return Int 'All others are INT
                End If
            End If

            ' FLOAT [op] FLOAT
            If L Is Float AndAlso R Is Float Then
                Return Float
            End If

            ' INT [op] FLOAT  |  FLOAT [op] INT
            If (L Is Int AndAlso R Is Float) OrElse (L Is Float AndAlso R Is Int) Then
                Return Float
            End If

            Return Nothing
        End Function

        Public Overrides Function CompileExpression(Scope As Context.Scope) As String
            Dim leftType As TypeSystem.Type = Left.GetExpressionReturnType(Scope)
            Dim rightType As TypeSystem.Type = Right.GetExpressionReturnType(Scope)

            ' Try shortcut
            Dim fastCompiled As String = TryHardcodedCompilation(leftType, rightType, Scope)
            If fastCompiled IsNot Nothing Then
                Return fastCompiled
            End If

            ' Fallback to relations
            Return GetRelation(leftType, rightType).CompileCall(Left, {Right}, Scope, Location)
        End Function

        Private Function TryHardcodedCompilation(L As TypeSystem.Type, R As TypeSystem.Type, Scope As Context.Scope) As String
            ' All operations compiles the same way -> just check if this is between known types (int, float)
            Dim isIntOp = (L Is Int AndAlso R Is Int)
            Dim isFloatOp = (L Is Float AndAlso R Is Float)
            Dim isMixedOp = (L Is Int AndAlso R Is Float) OrElse (L Is Float AndAlso R Is Int)

            If isIntOp OrElse isFloatOp OrElse isMixedOp Then
                Dim opSym As String = GetOperatorSymbol(Op)

                If opSym IsNot Nothing Then
                    Dim lCode = Left.CompileExpression(Scope)
                    Dim rCode = Right.CompileExpression(Scope)
                    Return $"({lCode} {opSym} {rCode})"
                End If
            End If

            Return Nothing
        End Function

        Private Function GetOperatorSymbol(operation As TypeSystem.RelationType) As String
            Select Case operation
                Case TypeSystem.RelationType.RELATION_ADD : Return "+"
                Case TypeSystem.RelationType.RELATION_SUB : Return "-"
                Case TypeSystem.RelationType.RELATION_MULT : Return "*"
                Case TypeSystem.RelationType.RELATION_DIV : Return "/"
                Case TypeSystem.RelationType.RELATION_MODULO : Return "%"
                Case Else : Return Nothing
            End Select
        End Function

    End Class
End Namespace