Namespace AST
    Public Class BracketsExpression
        Inherits ExpressionNode
        Implements IAssignable

        Private Target As ExpressionNode
        Private Arguments As IEnumerable(Of ExpressionNode)

        Public Sub New(Target As ExpressionNode, Arguments As IEnumerable(Of ExpressionNode), Location As Location)
            MyBase.New(Location)
            Me.Target = Target
            Me.Arguments = Arguments
        End Sub

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type
            Return Target.GetExpressionReturnType(Context).GetRelation(TypeSystem.RelationType.RELATION_BRACKETS, Arguments.Select(Function(arg) arg.GetExpressionReturnType(Context)), Location).ReturnType
        End Function

        Public Overrides Function CompileExpression(Scope As Context.Scope) As String
            Return Target.GetExpressionReturnType(Scope).GetRelation(TypeSystem.RelationType.RELATION_BRACKETS, Arguments.Select(Function(arg) arg.GetExpressionReturnType(Scope)), Location).CompileCall(Target, Arguments, Scope, Location)
        End Function

        Public Sub CompileAssignation(NewValue As ExpressionNode, Scope As Context.Scope) Implements IAssignable.CompileAssignation
            Dim TargetType As TypeSystem.Type = Target.GetExpressionReturnType(Scope)
            If TypeOf TargetType Is TypeSystem.RackType Then
                If Arguments.Count > 1 Then
                    Throw New SyntaxError("Only one index was expected here.", Arguments(1).Location)
                ElseIf Arguments.Count < 1 Then
                    Throw New SyntaxError("A index was expected here.", Arguments(1).Location)
                End If
                DirectCast(TargetType, TypeSystem.RackType).WriteElementAssignation(Target, Arguments.First, NewValue, Scope)
            Else
                Scope.WriteLine(TargetType.GetRelation(TypeSystem.RelationType.RELATION_SET_BRACKETS, Arguments.Select(Function(arg) arg.GetExpressionReturnType(Scope)).Append(NewValue.GetExpressionReturnType(Scope)), Location).CompileCall(Target, Arguments.Append(NewValue), Scope, Location) & ";")
            End If
        End Sub

    End Class
End Namespace