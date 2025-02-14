Namespace Lim

    Public Class StructureFieldGetterInvoker

        Inherits Lim.GetterInvoker

        Private StructureField_CompiledName As String

        Public Sub New(GetterReturnType As Lim.Type, FieldCompiledName As String)
            MyBase.New(GetterReturnType)
            Me.StructureField_CompiledName = FieldCompiledName
        End Sub

        Public Overrides Function CompileCall(Obj As String) As String
            Return $"{Obj}.{StructureField_CompiledName}"
        End Function
    End Class

End Namespace