Namespace AST
    Public Class FunctionCallExpression
        Inherits ExpressionNode

        Private Target As ExpressionNode
        Private PassedArguments As IEnumerable(Of ExpressionNode)

        Public Sub New(Target As ExpressionNode, PassedArguments As IEnumerable(Of ExpressionNode), Location As Location)
            MyBase.New(Location)
            Me.Target = Target
            Me.PassedArguments = PassedArguments
        End Sub

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type

            Dim FunctionType As TypeSystem.Type = Target.GetExpressionReturnType(Context)
            If TypeOf FunctionType IsNot TypeSystem.FunType Then
                Throw New TypeMismatchError("fun", FunctionType.ToString(), Location)
            End If

            Return DirectCast(FunctionType, TypeSystem.FunType).ReturnType

        End Function

        Public Overrides Function CompileExpression(Scope As Context.Scope) As String

            Dim FunctionType As TypeSystem.Type = Target.GetExpressionReturnType(Scope)
            If TypeOf FunctionType IsNot TypeSystem.FunType Then
                Throw New TypeMismatchError("fun", FunctionType.ToString(), Location)
            End If

            Return DirectCast(FunctionType, TypeSystem.FunType).ExecuteProcedure(Scope, Target.CompileExpression(Scope), PassedArguments)

        End Function

    End Class
End Namespace