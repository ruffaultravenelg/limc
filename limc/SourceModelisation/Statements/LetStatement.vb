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

            'If there is not value
            If VariableValue Is Nothing Then
                If Scope.WriteVariableDeclaration(VariableName, VariableType.GetTargetedType(Scope)) Is Nothing Then
                    Throw New SyntaxException("A variable with the same name was already declared in this scope.", Location)
                End If
                Return
            End If

            'If there is not type
            If VariableType Is Nothing Then
                If Scope.WriteVariableDeclaration(VariableName, VariableValue.GetReturnType(Scope), VariableValue.Compile(Scope)) Is Nothing Then
                    Throw New SyntaxException("A variable with the same name was already declared in this scope.", Location)
                End If
                Return
            End If

            'If there is a type and a value
            Dim SpecifiedType As Lim.Type = VariableType.GetTargetedType(Scope)
            Dim ValueType As Lim.Type = VariableValue.GetReturnType(Scope)

            'Check if the value type is compatible with the specified type
            If Not ValueType = SpecifiedType Then
                Throw New SyntaxException("The value type is not compatible with the specified type.", Location)
            End If

            'Write the variable declaration
            If Scope.WriteVariableDeclaration(VariableName, SpecifiedType, VariableValue.Compile(Scope)) Is Nothing Then
                Throw New SyntaxException("A variable with the same name was already declared in this scope.", Location)
            End If

        End Sub

        'Contains return statement
        Public Overrides ReadOnly Property ContainsReturnStatement As Boolean = False

    End Class

End Namespace