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

        'Search variables
        For Each UpperScope As Scope In Scope.Uppers
            For Each Variable As Variable In UpperScope.Variables
                If Variable.Name = VariableName AndAlso NewValue.CanReturn(Variable.Type, Scope) Then
                    Scope.WriteVariableAssignation(Variable, NewValue.Compile(Scope, Variable.Type))
                    Exit Sub
                End If
            Next
        Next

        'Propertie
        If Scope.HasAttachedClass Then
            For Each Propertie As Propertie In Scope.AttachedClass.Properties
                If Propertie.Name = VariableName AndAlso NewValue.CanReturn(Propertie.Type, Scope) Then
                    Scope.WritePropertieAssignation(Propertie, NewValue.Compile(Scope, Propertie.Type))
                    Exit Sub
                End If
            Next
        End If

    End Sub

End Class
