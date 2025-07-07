
Namespace Lim

    Public Class StructureFieldGetter
        Implements Lim.IGetter

        Public ReadOnly Property Name As String Implements IGetter.Name
        Public ReadOnly Property Type As Type Implements IGetter.Type

        Private Field_CompiledName As String

        Public Sub New(Name As String, Type As Lim.Type, Field_CompiledName As String)
            Me.Name = Name
            Me.Type = Type
            Me.Field_CompiledName = Field_CompiledName
        End Sub

        Public Function CompileCall(Scope As Scope, ParentObject As ExpressionNode) As String Implements IGetter.CompileCall
            Return $"{ParentObject.Compile(Scope)}.{Field_CompiledName}"
        End Function
    End Class

End Namespace