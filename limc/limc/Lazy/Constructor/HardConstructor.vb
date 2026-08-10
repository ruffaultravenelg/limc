Imports limc.CodeGen

Namespace Lazy
    Public Class HardConstructor
        Inherits Lazy.Constructor

        Public Overrides ReadOnly Property ArgumentTypes As IEnumerable(Of TypeSystem.Type)
        Private Body As IEnumerable(Of String)

        Public Sub New(AssociatedType As TypeSystem.Type, ArgumentTypes As IEnumerable(Of TypeSystem.Type), Body As IEnumerable(Of String))
            MyBase.New(AssociatedType)
            Me.ArgumentTypes = ArgumentTypes
            Me.Body = Body
        End Sub

        Protected Overrides Function GenerateCompiledMethod() As ContextedFunction
            Dim Writer As New CWriter()

            ' Create self
            If TypeOf AssociatedType Is TypeSystem.ClassType Then
                Writer.WriteLine($"{AssociatedType.cRepresentation} {INSTANCE_ARGUMENT_NAME} = {Constants.LIM_ALLOC}({AssociatedType.cSize});")
                Writer.WriteLine($"if ({INSTANCE_ARGUMENT_NAME} == NULL) {CodeGen.WritePanicCall("""Not enough memory""")};")

            ElseIf TypeOf AssociatedType Is TypeSystem.StructType Then
                Writer.WriteLine($"{AssociatedType.cRepresentation} {INSTANCE_ARGUMENT_NAME} = {AssociatedType.DefaultValue(New Context.Scope(Nothing, Nothing))};")

            Else
                Throw New InternalError()
            End If

            ' Add body
            Writer.WriteLines(Body)

            ' Add return self
            Writer.WriteLine($"return {INSTANCE_ARGUMENT_NAME};")

            Return New CodeGen.ContextedFunction(
                ArgumentTypes.Select(Function(a, i) $"{a.cRepresentation} arg{i}"),
                AssociatedType.cRepresentation,
                Writer.GetLines(),
                $"{AssociatedType.ToString()}.new"
            )
        End Function

        Protected Overrides Sub CompileBody()

        End Sub

    End Class
End Namespace