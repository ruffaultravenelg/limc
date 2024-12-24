Public MustInherit Class ExpressionNode
    Inherits Node

    'Constructor
    Public Sub New(Location As Location)
        MyBase.New(Location)
    End Sub

    'Get the return type of the expression
    Public MustOverride Function GetReturnType(Context As Context) As Lim.Type

    'Compile the expression
    Public MustOverride Function Compile(Scope As Scope) As String

End Class
