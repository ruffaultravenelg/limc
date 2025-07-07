
Namespace Lim

    Public Class StructureFieldSetter
        Implements Lim.ISetter

        Public ReadOnly Property Name As String Implements ISetter.Name
        Public ReadOnly Property Type As Type Implements ISetter.Type

        Private Field_CompiledName As String

        Public Sub New(Name As String, Type As Lim.Type, Field_CompiledName As String)
            Me.Name = Name
            Me.Type = Type
            Me.Field_CompiledName = Field_CompiledName
        End Sub

        Public Sub CompileCall(Scope As Scope, ParentObject As ExpressionNode, NewValue As ExpressionNode) Implements ISetter.CompileCall
            Scope.WriteLine($"{ParentObject.Compile(Scope)}.{Field_CompiledName} = {NewValue.Compile(Scope)};")
        End Sub
    End Class

End Namespace