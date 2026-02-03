Namespace AST
    Public Class DeclareConstantWithValueStatement
        Inherits StatementNode

        Private ConstantName As String
        Private ConstantValue As AST.ExpressionNode

        Public Sub New(ConstantName As String, ConstantValue As AST.ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.ConstantName = ConstantName
            Me.ConstantValue = ConstantValue
        End Sub

        Public Overrides Sub Compile(Writer As CWriter, Scope As Context.Scope)

            Dim Type As TypeSystem.Type = ConstantValue.GetExpressionReturnType(Scope)
            Dim ConstantInfo As ConstantData = Scope.CreateConstant(ConstantName, Type, Location)

            If Type.cRepresentation.Contains("*") Then
                Writer.WriteLine($"{Type.cRepresentation} const {ConstantInfo.CompiledName} = {ConstantValue.CompileExpression(Writer, Scope)};")
            Else
                Writer.WriteLine($"const {Type.cRepresentation} {ConstantInfo.CompiledName} = {ConstantValue.CompileExpression(Writer, Scope)};")
            End If

        End Sub

    End Class

End Namespace