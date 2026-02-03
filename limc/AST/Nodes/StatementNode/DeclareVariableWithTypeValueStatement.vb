Namespace AST
    Public Class DeclareVariableWithTypeValueStatement
        Inherits StatementNode

        Private VariableName As String
        Private VariableType As AST.TypeNode
        Private VariableValue As AST.ExpressionNode

        Public Sub New(VariableName As String, VariableType As AST.TypeNode, VariableValue As AST.ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.VariableName = VariableName
            Me.VariableType = VariableType
            Me.VariableValue = VariableValue
        End Sub

        Public Overrides Sub Compile(Writer As CWriter, Scope As Context.Scope)

            Dim WantedType As TypeSystem.Type = VariableType.GetAssociatedType(Scope)
            Dim ValueType As TypeSystem.Type = VariableValue.GetExpressionReturnType(Scope)

            If Not WantedType = ValueType Then
                Throw New SyntaxError("Type mismatch in variable declaration. Expected " & WantedType.ToString() & " but got " & ValueType.ToString() & ".", VariableValue.Location)
            End If

            Dim VariableInfo As VariableData = Scope.CreateVariable(VariableName, WantedType, Location)
            Writer.WriteLine($"{WantedType.cRepresentation} {VariableInfo.CompiledName} = {VariableValue.CompileExpression(Writer, Scope)};")

        End Sub

    End Class

End Namespace