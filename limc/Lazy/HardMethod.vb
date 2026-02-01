
Namespace Lazy
    Public Class HardMethod
        Inherits Method

        Private Body As IEnumerable(Of String)

        Public Sub New(ParentType As TypeSystem.Type, Name As String, ArgumentTypes As IEnumerable(Of TypeSystem.Type), ReturnType As TypeSystem.Type, Body As IEnumerable(Of String))
            MyBase.New(ParentType)
            Me.Name = Name
            Me.ArgumentTypes = ArgumentTypes
            Me.ReturnType = ReturnType
            Me.Body = Body
        End Sub

        Public Overrides ReadOnly Property Name As String
        Public Overrides ReadOnly Property ArgumentTypes As IEnumerable(Of TypeSystem.Type)
        Public Overrides ReadOnly Property ReturnType As TypeSystem.Type

        Protected Overrides Function CompileGeneratedFunction() As CodeGen.PassingContextFunction
            Return New CodeGen.PassingContextFunction(
                ArgumentTypes.Select(Function(a, i) $"{a.cRepresentation} arg{i}").Prepend($"{ParentType.cRepresentation} {Constants.INSTANCE_ARGUMENT_NAME}"),
                If(ReturnType Is Nothing, "void", ReturnType.cRepresentation),
                Body,
                $"{ParentType.ToString()}.{Name}"
            )
        End Function

    End Class
End Namespace