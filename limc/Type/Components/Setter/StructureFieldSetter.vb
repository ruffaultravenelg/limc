Namespace Lim

    Public Class StructureFieldSetterInvoker

        Inherits Lim.SetterInvoker

        Private StructureField_CompiledName As String

        Public Sub New(SetterValueType As Lim.Type, FieldCompiledName As String)
            MyBase.New(SetterValueType)
            Me.StructureField_CompiledName = FieldCompiledName
        End Sub

        Public Overrides Sub CompileAssignation(Scope As Scope, Obj As String, NewValue As String)
            Scope.WriteLine($"{Obj}.{StructureField_CompiledName} = {NewValue};")
        End Sub

    End Class

End Namespace