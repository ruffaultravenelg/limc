Namespace Source

    Public Class LetStatement
        Inherits StatementNode

        Private VariableName As String
        Private VariableType As Source.Type
        Private VariableValue As ExpressionNode

        Public Sub New(Location As Location, VariableName As String, VariableType As Source.Type, VariableValue As ExpressionNode)
            MyBase.New(Location)
            Me.VariableName = VariableName
            Me.VariableType = VariableType
            Me.VariableValue = VariableValue

            'If there is not provided type or value -> impossible to determine type
            If VariableType Is Nothing AndAlso VariableValue Is Nothing Then
                Throw New SyntaxException("Impossible to determine type of variable, please add a type or a default value.", Location)
            End If

        End Sub

    End Class

End Namespace