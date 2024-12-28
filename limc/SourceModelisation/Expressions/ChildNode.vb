Namespace Source
    Public Class ChildNode
        Inherits ExpressionNode

        'Value
        Private Parent As ExpressionNode
        Private Propertie As String

        'Constructor
        Public Sub New(Location As Location, Parent As ExpressionNode, Propertie As String)
            MyBase.New(Location)
            Me.Parent = Parent
            Me.Propertie = Propertie
        End Sub

        'Get the return type
        Public Overrides Function GetReturnType(Context As Context) As Lim.Type

            'Get type
            Dim ParentType As Lim.Type = Parent.GetReturnType(Context)

            'No getter
            If Not ParentType.HasGetter(Propertie) Then
                Throw New SyntaxException($"The ""{ParentType}"" type has no getter named ""{Propertie}"".", Location)
            End If

            'Return getter type
            Return ParentType.GetGetterType(Propertie)

        End Function

        'Compile the expression
        Public Overrides Function Compile(Scope As Scope) As String

            'Get type
            Dim ParentType As Lim.Type = Parent.GetReturnType(Scope)

            'No getter
            If Not ParentType.HasGetter(Propertie) Then
                Throw New SyntaxException($"The ""{ParentType}"" type has no getter named ""{Propertie}"".", Location)
            End If

            'Return getter type
            Return ParentType.CallGetter(Propertie, Parent.Compile(Scope))

        End Function

    End Class

End Namespace