Namespace AST
    Public Class DeclareVariableWithValueStatement
        Inherits StatementNode

        Private VariableName As String
        Private VariableValue As AST.ExpressionNode

        Public Sub New(VariableName As String, VariableValue As AST.ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.VariableName = VariableName
            Me.VariableValue = VariableValue
        End Sub

        Public Overrides Sub Compile(Writer As CWriter, Scope As Context.Scope)

            Dim Type As TypeSystem.Type = VariableValue.GetExpressionReturnType(Scope)
            Dim VariableInfo As VariableData = Scope.CreateVariable(VariableName, Type, Location)

            Writer.WriteLine($"{Type.cRepresentation} {VariableInfo.CompiledName} = {VariableValue.CompileExpression(Writer, Scope)};")

        End Sub

    End Class

End Namespace