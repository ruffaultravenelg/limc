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
            If Not TargetType.Setters.HasSetter(Target.Propertie) Then
                Throw New SyntaxException($"the ""{TargetType.ToString()}"" type does not have a setter named ""{Target.Propertie}"".", Target.Location)
            End If

            'Test types
            Dim NewValueType As Lim.Type = NewValue.GetReturnType(Scope)
            Dim ExpectedType As Lim.Type = TargetType.Setters.GetSetterType(Target.Propertie)
            If Not ExpectedType = NewValueType Then
                Throw New TypeException($"the ""{ExpectedType.ToString()}"" type was expected instead of a ""{NewValueType.ToString()}"".", NewValue.Location)
            End If

            'Compile
            TargetType.Setters.CompileAssignation(Target.Propertie, Scope, Target.Parent.Compile(Scope), NewValue.Compile(Scope))

        End Sub

        'Contains return statement
        Public Overrides ReadOnly Property ContainsReturnStatement As Boolean = False

    End Class

End Namespace