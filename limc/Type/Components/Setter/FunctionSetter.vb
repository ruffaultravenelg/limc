Namespace Lim
    Public Class FunctionSetter
        Inherits Lim.SetterInvoker


        Private CompiledSetterFunctionName As String

        Public Sub New(SetterReturnType As Lim.Type, CompiledSetterFunctionName As String)
            MyBase.New(SetterReturnType)
            Me.CompiledSetterFunctionName = CompiledSetterFunctionName
        End Sub

        Public Overrides Sub CompileAssignation(Scope As Scope, Obj As String, NewValue As String)
            Scope.WriteLine(C.Function.WriteCall(CompiledSetterFunctionName, Obj, NewValue) & ";")
        End Sub

    End Class


End Namespace