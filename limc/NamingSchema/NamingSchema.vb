Public MustInherit Class NamingSchema

    Public MustOverride Function GenerateTypeName() As String
    Public MustOverride Function GenerateFunctionName() As String
    Public MustOverride Function GenerateVariableName() As String

End Class
