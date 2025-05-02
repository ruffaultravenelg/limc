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

    'Get the types of a list of expressions 
    Public Shared Function GetTypesOfExpressions(Expressions As IEnumerable(Of ExpressionNode), Context As Context) As IEnumerable(Of Lim.Type)
        Dim Types As New List(Of Lim.Type)
        For Each Expr As ExpressionNode In Expressions
            Types.Add(Expr.GetReturnType(Context))
        Next
        Return Types
    End Function

End Class

