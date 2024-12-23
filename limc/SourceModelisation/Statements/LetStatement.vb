Namespace Source

    Public Class LetStatement
        Inherits StatementNode

        'Properties
        Private VariableName As String
        Private VariableType As Source.Type
        Private VariableValue As ExpressionNode

        'Constructor
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

        'Compile
        Public Overrides Sub Compile(Scope As Scope)



        End Sub

    End Class

End Namespace