Namespace TypeSystem

    Public MustInherit Class Type

        Public Shared ReadOnly Property Int As IntType = New IntType()
        Public Shared ReadOnly Property Str As StrType = New StrType()
        Public Shared ReadOnly Property Bool As BoolType = New BoolType()

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

        Protected MustOverride ReadOnly Property Relations As IEnumerable(Of Lazy.Relation)
        Public Function GetRelation(Type As RelationType, ArgumentsTypes As IEnumerable(Of Type), Location As Location) As Lazy.Relation
            For Each Relation In Relations

                ' Check type
                If Relation.Type <> Type Then
                    Continue For
                End If

                ' Check arguments
                If ArgumentsTypes.Count <> Relation.ArgumentsTypes.Count Then
                    Continue For
                End If
                Dim AllGood As Boolean = True
                For i = 0 To ArgumentsTypes.Count - 1
                    If Relation.ArgumentsTypes(i) <> ArgumentsTypes(i) Then
                        AllGood = False
                        Exit For
                    End If
                Next

                If Not AllGood Then
                    Continue For
                End If

                Return Relation

            Next
            Throw New SyntaxError($"The ""{ToString()}"" type does not contain such a relation.", Location)
        End Function

    End Class

End Namespace