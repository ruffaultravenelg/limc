Namespace TypeSystem

    Public MustInherit Class Type

        Public Shared ReadOnly Property Int As IntType = New IntType()

        Public MustOverride ReadOnly Property cRepresentation As String
        Public MustOverride Function DefaultValue(Scope As Scope) As String

        Public Overridable Sub SetVariableValue(Scope As Scope, Variable As String, NewValue As String)
            Scope.WriteLine($"{Variable} = {NewValue};")
        End Sub

        Public Shared Operator =(a As Type, b As Type) As Boolean
            Return a.cRepresentation = b.cRepresentation
        End Operator
        Public Shared Operator <>(a As Type, b As Type) As Boolean
            Return Not a.cRepresentation = b.cRepresentation
        End Operator

    End Class

End Namespace