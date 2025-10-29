Namespace TypeSystem

    Public MustInherit Class Type

        Public Shared ReadOnly Property Int As IntType = New IntType()
        Public Shared ReadOnly Property Str As StrType = New StrType()

        Public MustOverride ReadOnly Property cRepresentation As String
        Public MustOverride Function DefaultValue(Scope As Context.Scope) As String

        Public Overridable Sub SetVariableValue(Scope As Context.Scope, Variable As String, NewValue As String)
            Scope.WriteLine($"{Variable} = {NewValue};")
        End Sub

        Public Shared Operator =(a As Type, b As Type) As Boolean
            If a Is Nothing AndAlso b Is Nothing Then
                Return True
            ElseIf a Is Nothing OrElse b Is Nothing Then
                Return False
            Else
                Return a.cRepresentation = b.cRepresentation
            End If
        End Operator
        Public Shared Operator <>(a As Type, b As Type) As Boolean
            Return Not a = b
        End Operator

        Public MustOverride Function RetrieveElements(Name As String) As IEnumerable(Of SearchMatch)

    End Class

End Namespace