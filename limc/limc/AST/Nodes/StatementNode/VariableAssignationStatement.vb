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

        Public Overrides Sub Compile(Writer As CWriter, Scope As Context.Scope)
            Target.CompileAssignation(NewValue, Writer, Scope)
        End Sub

    End Class
End Namespace