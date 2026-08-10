Namespace AST
    Public Class NewExpression
        Inherits ExpressionNode

        Private Type As TypeNode
        Private Arguments As IEnumerable(Of ExpressionNode)

        Public Sub New(Type As TypeNode, Arguments As IEnumerable(Of ExpressionNode), Location As Location)
            MyBase.New(Location)
            Me.Type = Type
            Me.Arguments = Arguments
        End Sub

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type
            Return Type.GetAssociatedType(Context)
        End Function

        Public Overrides Function CompileExpression(Writer As CWriter, Scope As Context.Scope) As String

            ' Get class
            Dim AssociatedType As TypeSystem.Type = Type.GetAssociatedType(Scope)
            If TypeOf AssociatedType IsNot TypeSystem.ClassType Then
                Throw New SyntaxError($"{AssociatedType.ToString()} is not a class, therefore it cannot be instanciated if new()", Location)
            End If

            Dim AssociatedClass As TypeSystem.ClassType = AssociatedType

            ' Compile types
            Dim ArgumentTypes = Arguments.Select(Function(arg) arg.GetExpressionReturnType(Scope))

            ' Get constructor
            Dim Constructor As Lazy.Constructor = AssociatedClass.GetConstructor(ArgumentTypes)
            If Constructor Is Nothing Then
                Throw New UnknownOrUnreachableConstructorError(AssociatedClass, ArgumentTypes, Location)
            End If

            ' Compile constructor call
            Return Constructor.CompileCall(Arguments, Writer, Scope, Location)

        End Function

    End Class
End Namespace