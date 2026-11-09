Imports limc.Symbol

Namespace Environment
    '
    ' Used in HIR.Lowerer.ExpressionCompiler visitor
    ' Used by visitProcedureCall to hint the arguments of the call types to the procedure.accept()
    '      "just" ElementExpression cannot resolve wich function used (function overload)
    '
    Friend Class CallHintEnv
        Inherits Env

        Private TargetedNode As AST.Node
        Private ArgumentTypes As IReadOnlyList(Of TypeSymbol)

        Public Sub New(Parent As Env, TargetedNode As AST.Node, ArgumentTypes As IEnumerable(Of TypeSymbol))
            MyBase.New(Parent)
            Me.TargetedNode = TargetedNode
            Me.ArgumentTypes = ArgumentTypes.ToList()
        End Sub

        ' is hint if
        '   ctx is CallHintEnv
        '   checking node is the one targeted by who created the hint
        ' helps to stop hint propagation after the targeted node
        Public Shared Function IsHintFor(Ctx As Env, CurrentNode As AST.Node) As Boolean
            Return TypeOf Ctx Is CallHintEnv AndAlso Object.ReferenceEquals(DirectCast(Ctx, CallHintEnv).TargetedNode, CurrentNode)
        End Function
        Public Shared Function GetHintedTypes(Ctx As Env, CurrentNode As AST.Node) As IReadOnlyList(Of TypeSymbol)
            If IsHintFor(Ctx, CurrentNode) Then
                Return DirectCast(Ctx, CallHintEnv).ArgumentTypes
            Else
                Throw New InternalException("Cannot get hinted types of a non hint env")
            End If
        End Function


        ' ==== NOT USED HERE ====
        Protected Overrides Iterator Function ResolveSymbol(ElementName As String, PassedGenericTypes As IReadOnlyList(Of Symbol.TypeSymbol)) As IEnumerable(Of SymbolMatch)
            ' This environment doesn't contains any symbols (other than type)
        End Function

        Protected Overrides Function ResolveType(Typename As String, PassedGenericTypes As IReadOnlyList(Of TypeSymbol)) As TypeSymbol
            Return Nothing
        End Function

    End Class
End Namespace