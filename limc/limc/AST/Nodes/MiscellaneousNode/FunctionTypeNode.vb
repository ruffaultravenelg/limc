Namespace AST
    Public Class FunctionTypeNode
        Inherits AST.TypeNode

        Private ReadOnly FunctionArgumentTypes As IEnumerable(Of AST.TypeNode)
        Private ReadOnly FunctionReturnType As AST.TypeNode 'nullable

        Public Sub New(FunctionArgumentTypes As IEnumerable(Of AST.TypeNode), FunctionReturnType As AST.TypeNode, Location As Location)
            MyBase.New(Location)
            Me.FunctionArgumentTypes = FunctionArgumentTypes
            Me.FunctionReturnType = FunctionReturnType
        End Sub

        Public Overrides Function GetAssociatedType(Context As Context.Context) As TypeSystem.Type
            Return TypeSystem.FunType.From(
                FunctionArgumentTypes.Select(Function(a) a.GetAssociatedType(Context)),
                If(FunctionReturnType Is Nothing, Nothing, FunctionReturnType.GetAssociatedType(Context))
            )
        End Function

    End Class
End Namespace