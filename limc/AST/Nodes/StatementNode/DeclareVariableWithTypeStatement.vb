Namespace AST
    Public Class DeclareVariableWithTypeStatement
        Inherits StatementNode

        Private VariableName As String
        Private VariableType As AST.TypeNode

        Public Sub New(VariableName As String, VariableType As AST.TypeNode, Location As Location)
            MyBase.New(Location)
            Me.VariableName = VariableName
            Me.VariableType = VariableType
        End Sub

        Public Overrides Sub Compile(Writer As CWriter, Scope As Context.Scope)

            Dim Type As TypeSystem.Type = VariableType.GetAssociatedType(Scope)
            Dim VariableInfo As VariableData = Scope.CreateVariable(VariableName, Type, Location)

            Writer.WriteLine($"{Type.cRepresentation} {VariableInfo.CompiledName} = {Type.DefaultValue(Scope)};")

        End Sub

    End Class

End Namespace