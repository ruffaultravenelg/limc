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

End Class
