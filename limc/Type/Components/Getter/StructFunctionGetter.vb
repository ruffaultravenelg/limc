Namespace Lim

    Public Class StructFunctionGetter

        Inherits Lim.GetterInvoker

        Private CompiledGetterFunctionName As String

        Public Sub New(GetterReturnType As Lim.Type, CompiledGetterFunctionName As String)
            MyBase.New(GetterReturnType)
            Me.CompiledGetterFunctionName = CompiledGetterFunctionName
        End Sub

        Public Overrides Function CompileCall(Obj As String) As String
            Return C.Function.WriteCall(CompiledGetterFunctionName, Obj)
        End Function
    End Class

End Namespace