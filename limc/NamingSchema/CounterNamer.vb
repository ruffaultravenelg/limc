Public Class CounterNamer
    Inherits NamingSchema

    'Variable
    Private VariableCount As Integer = 0
    Public Overrides Function GenerateVariableName() As String
        VariableCount += 1
        Return "var" & VariableCount
    End Function

    'Function
    Private FunctionCount As Integer = 0
    Public Overrides Function GenerateFunctionName() As String
        FunctionCount += 1
        Return "function" & FunctionCount
    End Function

    'Type
    Private TypeCount As Integer = 0
    Public Overrides Function GenerateTypeName() As String
        TypeCount += 1
        Return "type" & TypeCount
    End Function

    'Getter
    Private GetterCount As Integer = 0
    Public Overrides Function GenerateGetterName() As String
        GetterCount += 1
        Return "getter" & GetterCount
    End Function

    'Setter
    Private SetterCount As Integer = 0
    Public Overrides Function GenerateSetterName() As String
        SetterCount += 1
        Return "setter" & SetterCount
    End Function

    'Field
    Private FieldCount As Integer = 0
    Public Overrides Function GenerateFieldName() As String
        FieldCount += 1
        Return "field" & FieldCount
    End Function

End Class
