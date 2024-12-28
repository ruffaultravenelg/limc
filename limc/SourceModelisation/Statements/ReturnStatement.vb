Namespace Source

    Public Class ReturnStatement
        Inherits StatementNode

        'Properties
        Private Value As ExpressionNode

        'Constructor
        Public Sub New(Location As Location, Value As ExpressionNode)
            MyBase.New(Location)
            Me.Value = Value
        End Sub

        'Compile
        Public Overrides Sub Compile(Scope As Scope)

            'Get return type
            Dim ValueType As Lim.Type = Value.GetReturnType(Scope)

            'Check with current function return type
            Dim ReturnableScope As ReturnableScope = Scope.ReturnableScope

            'No return scope -> error
            If ReturnableScope Is Nothing Then
                Throw New SyntaxException("Cannot use a return statement here", Location)
            End If

            'Compare types
            If Not ReturnableScope.CanIReturn(ValueType) Then
                Throw New SyntaxException($"The expected type was ""{ReturnableScope.ConstructReturnType}"" but the supplied type is ""{ValueType}""", Location)
            End If

            'Compile return
            Scope.WriteReturn(Value.Compile(Scope))

        End Sub

        'Contains return statement
        Public Overrides ReadOnly Property ContainsReturnStatement As Boolean = True

    End Class

End Namespace