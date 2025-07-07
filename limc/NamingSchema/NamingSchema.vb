Public MustInherit Class NamingSchema

    Public MustOverride Function GenerateTypeName() As String
    Public MustOverride Function GenerateFunctionName() As String
    Public MustOverride Function GenerateVariableName() As String
    Public MustOverride Function GenerateTempName() As String
    Public MustOverride Function GenerateGetterName() As String
    Public MustOverride Function GenerateSetterName() As String
    Public MustOverride Function GenerateFieldName() As String

End Class
