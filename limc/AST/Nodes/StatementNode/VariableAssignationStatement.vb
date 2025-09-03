Namespace AST
    Public Class VariableAssignationStatement
        Inherits StatementNode

        Private Target As IAssignable
        Private NewValue As ExpressionNode

        Public Sub New(Target As IAssignable, NewValue As ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.Target = Target
            Me.NewValue = NewValue
        End Sub

        Public Overrides Sub Compile(Scope As Scope)
            Target.CompileAssignation(NewValue, Scope)
        End Sub

    End Class
End Namespace