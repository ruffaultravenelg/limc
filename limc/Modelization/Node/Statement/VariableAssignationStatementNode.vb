Public Class VariableAssignationStatementNode
    Inherits StatementNode

    '===================================
    '======== TARGETED VARIABLE ========
    '===================================
    Private VariableName As String

    '===========================
    '======== NEW VALUE ========
    '===========================
    Private NewValue As ExpressionNode

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, VariableName As String, NewValue As ExpressionNode)
        MyBase.New(Location)

        'Set values
        Me.VariableName = VariableName
        Me.NewValue = NewValue

    End Sub

    '=========================
    '======== COMPILE ========
    '=========================
    Public Overrides Sub Compile(Scope As Scope)

        'Contains results
        Dim FoundCount As Integer = 0
        Dim VariableType As Type = Nothing

        'Search variables
        For Each UpperScope As Scope In Scope.Uppers
            For Each Variable As Variable In UpperScope.Variables
                If Variable.Name = VariableName Then

                    'Write assignation
                    If NewValue.CanReturn(Variable.Type, Scope) Then
                        Scope.WriteVariableAssignation(Variable, NewValue.Compile(Scope, Variable.Type))
                        Exit Sub
                    End If

                    'Add to total count for err message
                    FoundCount += 1
                    VariableType = Variable.Type

                End If
            Next
        Next

        'Propertie
        If Scope.HasAttachedClass Then
            For Each Propertie As Propertie In Scope.AttachedClass.Properties
                If Propertie.Name = VariableName Then

                    'Write assignation
                    If NewValue.CanReturn(Propertie.Type, Scope) Then
                        Scope.WritePropertieAssignation(Propertie, NewValue.Compile(Scope, Propertie.Type))
                        Exit Sub
                    End If

                    'Add to total count for err message
                    FoundCount += 1
                    VariableType = Propertie.Type

                End If
            Next
        End If

        'Cannot find variable/propertie
        If FoundCount = 0 Then
            Throw New SyntaxErrorException("No """ & VariableName & """ variable or property can be found in this scope.", Me.Location)
        ElseIf FoundCount = 1 Then
            Throw New SyntaxErrorException("The given value is of type """ & NewValue.ReturnType(Scope).ToString() & """, but the variable """ & VariableName & """ can only contain values of type """ & VariableType.ToString() & """.", NewValue.Location)
        Else
            Throw New SyntaxErrorException("None of the variables/properties nameed """ & VariableName & """ can contain such a value.", NewValue.Location)
        End If

    End Sub

End Class
