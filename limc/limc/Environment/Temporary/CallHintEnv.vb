Imports limc.Symbol

Namespace Environment
    '
    ' Used in HIR.Lowerer.ExpressionCompiler visitor
    ' Used by visitProcedureCall to hint the arguments of the call types to the procedure.accept()
    '      "just" ElementExpression cannot resolve wich function used (function overload)
    '
    Friend Class CallHintEnv
        Inherits Env

        Private ArgumentTypes As IReadOnlyList(Of TypeSymbol)

        Public Sub New(Parent As Env, ArgumentTypes As IEnumerable(Of TypeSymbol))
            MyBase.New(Parent)
            Me.ArgumentTypes = ArgumentTypes.ToList()
        End Sub

        Public Shared Function IsHint(Ctx As Env) As Boolean
            Return TypeOf Ctx Is CallHintEnv
        End Function
        Public Shared Function GetHintedTypes(Ctx As Env) As IReadOnlyList(Of TypeSymbol)
            If IsHint(Ctx) Then
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