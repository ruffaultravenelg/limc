Namespace AST
    Public Class DeclareConstantWithTypeValueStatement
        Inherits StatementNode

        Private ConstantName As String
        Private ConstantType As AST.TypeNode
        Private ConstantValue As AST.ExpressionNode

        Public Sub New(ConstantName As String, ConstantType As AST.TypeNode, ConstantValue As AST.ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.ConstantName = ConstantName
            Me.ConstantType = ConstantType
            Me.ConstantValue = ConstantValue
        End Sub

        Public Overrides Sub Compile(Scope As Context.Scope)

            Dim WantedType As TypeSystem.Type = ConstantType.GetAssociatedType(Scope)
            Dim ValueType As TypeSystem.Type = ConstantValue.GetExpressionReturnType(Scope)

            If WantedType <> ValueType Then
                Throw New SyntaxError("Type mismatch in constant declaration. Expected " & WantedType.ToString() & " but got " & ValueType.ToString() & ".", ConstantValue.Location)
            End If

            Dim ConstantInfo As ConstantData = Scope.CreateConstant(ConstantName, WantedType, Location)

            If WantedType.cRepresentation.Contains("*") Then
                Scope.WriteLine($"{WantedType.cRepresentation} const {ConstantInfo.CompiledName} = {ConstantValue.CompileExpression(Scope)};")
            Else
                Scope.WriteLine($"const {WantedType.cRepresentation} {ConstantInfo.CompiledName} = {ConstantValue.CompileExpression(Scope)};")
            End If

        End Sub

    End Class

End Namespace