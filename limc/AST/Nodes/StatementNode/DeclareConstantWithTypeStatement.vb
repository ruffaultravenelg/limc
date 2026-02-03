Namespace AST
    Public Class DeclareConstantWithTypeStatement
        Inherits StatementNode

        Private ConstantName As String
        Private ConstantType As AST.TypeNode

        Public Sub New(ConstantName As String, ConstantType As AST.TypeNode, Location As Location)
            MyBase.New(Location)
            Me.ConstantName = ConstantName
            Me.ConstantType = ConstantType
        End Sub

        Public Overrides Sub Compile(Writer As CWriter, Scope As Context.Scope)

            Dim Type As TypeSystem.Type = ConstantType.GetAssociatedType(Scope)
            Dim ConstantInfo As ConstantData = Scope.CreateConstant(ConstantName, Type, Location)

            If Type.cRepresentation.Contains("*") Then
                Writer.WriteLine($"{Type.cRepresentation} const {ConstantInfo.CompiledName} = {Type.DefaultValue(Scope)};")
            Else
                Writer.WriteLine($"const {Type.cRepresentation} {ConstantInfo.CompiledName} = {Type.DefaultValue(Scope)};")
            End If

        End Sub

    End Class

End Namespace