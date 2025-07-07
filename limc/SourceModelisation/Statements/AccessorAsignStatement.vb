Namespace Source

    Public Class AccessorAssignStatement
        Inherits StatementNode

        'Properties
        Private Target As ChildNode
        Private NewValue As ExpressionNode

        'Constructor
        Public Sub New(Location As Location, Target As ChildNode, NewValue As ExpressionNode)
            MyBase.New(Location)
            Me.Target = Target
            Me.NewValue = NewValue
        End Sub

        'Compile
        Public Overrides Sub Compile(Scope As Scope)

            'Get type
            Dim TargetType As Lim.Type = Target.Parent.GetReturnType(Scope)

            'Has setter ?
            Dim Setter As Lim.ISetter = TargetType.Setter(Target.Propertie)
            If Setter Is Nothing Then
                Throw New SyntaxException($"the ""{TargetType.ToString()}"" type does not have a setter named ""{Target.Propertie}"".", Target.Location)
            End If

            'Test types
            Dim NewValueType As Lim.Type = NewValue.GetReturnType(Scope)
            If Not Setter.Type = NewValueType Then
                Throw New TypeException($"the ""{Setter.Type.ToString()}"" type was expected instead of a ""{NewValueType.ToString()}"".", NewValue.Location)
            End If

            'Compile
            Setter.CompileCall(Scope, Target.Parent, NewValue)

        End Sub

        'Contains return statement
        Public Overrides ReadOnly Property ContainsReturnStatement As Boolean = False

    End Class

End Namespace