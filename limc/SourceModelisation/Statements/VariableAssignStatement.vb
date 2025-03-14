Namespace Source

    Public Class VariableAssignStatement
        Inherits StatementNode

        'Properties
        Private VariableName As String
        Private NewValue As ExpressionNode

        'Constructor
        Public Sub New(Location As Location, VariableName As String, NewValue As ExpressionNode)
            MyBase.New(Location)
            Me.VariableName = VariableName
            Me.NewValue = NewValue
        End Sub

        'Compile
        Public Overrides Sub Compile(Scope As Scope)

            'Get variable
            Dim Variable As Lim.Variable = Scope.Variable(VariableName)
            If Variable Is Nothing Then
                Throw New SyntaxException($"No variable '{VariableName}' is accessible in this scope.", Location)
            End If

            'Compare types
            Dim ValueType As Lim.Type = NewValue.GetReturnType(Scope)
            If Not Variable.Type = ValueType Then
                Throw New SyntaxException($"The value type ({ValueType.ToString()}) doesn't match the variable type ({Variable.Type.ToString()})", Location)
            End If

            'Assign value
            Scope.WriteVariableAssignation(Variable, NewValue.Compile(Scope))

        End Sub

        'Contains return statement
        Public Overrides ReadOnly Property ContainsReturnStatement As Boolean = False

    End Class

End Namespace