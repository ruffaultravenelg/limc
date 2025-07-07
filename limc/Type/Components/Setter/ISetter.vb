Namespace Lim
    Public Interface ISetter

        ReadOnly Property Name As String
        ReadOnly Property Type As Lim.Type

        Sub CompileCall(Scope As Scope, ParentObject As ExpressionNode, NewValue As ExpressionNode)

    End Interface
End Namespace