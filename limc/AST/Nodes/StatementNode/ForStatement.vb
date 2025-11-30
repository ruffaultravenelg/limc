Imports limc.TypeSystem.Type
Namespace AST
    Public Class ForStatement
        Inherits StatementNode

        Private VariableName As String
        Private VariableType As TypeNode
        Private Value_From As ExpressionNode
        Private Value_To As ExpressionNode
        Private Instructions As IEnumerable(Of StatementNode)

        Public Sub New(VariableName As String, VariableType As TypeNode, Value_From As ExpressionNode, Value_To As ExpressionNode, Instructions As IEnumerable(Of StatementNode), Location As Location)
            MyBase.New(Location)
            Me.VariableName = VariableName
            Me.VariableType = VariableType
            Me.Value_From = Value_From
            Me.Value_To = Value_To
            Me.Instructions = Instructions
        End Sub

        Public Overrides Sub Compile(Scope As Context.Scope)

            ' Check only INT
            If Value_From IsNot Nothing AndAlso Value_From.GetExpressionReturnType(Scope) IsNot Int Then
                Throw New TypeMismatchError(Int, Value_From.GetExpressionReturnType(Scope), Value_From.Location)
            End If
            If Value_To.GetExpressionReturnType(Scope) IsNot Int Then
                Throw New TypeMismatchError(Int, Value_To.GetExpressionReturnType(Scope), Value_To.Location)
            End If
            If VariableType IsNot Nothing AndAlso VariableType.GetAssociatedType(Scope) IsNot Int Then
                Throw New TypeMismatchError(Int, VariableType.GetAssociatedType(Scope), VariableType.Location)
            End If

            ' Create scope & variable
            Dim LoopScope As New Context.LoopScope(Scope, Location)
            Dim IteratorVariable As VariableData = LoopScope.CreateVariable(VariableName, Int)

            ' Compile body
            For Each Statement In Instructions
                Statement.Compile(LoopScope)
            Next

            ' Compile header
            Scope.WriteLine($"for (int {IteratorVariable.CompiledName} = {If(Value_From IsNot Nothing, Value_From.CompileExpression(Scope), "0")}; {IteratorVariable.CompiledName} < {Value_To.CompileExpression(Scope)}; {IteratorVariable.CompiledName}++){{")
            Scope.WriteScope(LoopScope)
            Scope.WriteLine("}")

        End Sub

        Protected Overrides Function GetChildNodes() As IEnumerable(Of StatementNode)
            Return Instructions
        End Function

    End Class
End Namespace