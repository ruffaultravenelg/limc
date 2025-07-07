Namespace Lim
    Public Interface IGetter

        ReadOnly Property Name As String
        ReadOnly Property Type As Lim.Type

        Function CompileCall(Scope As Scope, ParentObject As ExpressionNode) As String

    End Interface
End Namespace