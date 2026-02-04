Namespace Lazy
    Public Class HardSetter
        Inherits Setter
        Public Overrides ReadOnly Property Name As String
        Public Overrides ReadOnly Property Type As TypeSystem.Type
        Private Body As IEnumerable(Of String)
        Private AssociatedType As TypeSystem.Type

        Public Sub New(AssociatedType As TypeSystem.Type, Name As String, Type As TypeSystem.Type, Body As IEnumerable(Of String))
            Me.AssociatedType = AssociatedType
            Me.Name = Name
            Me.Type = Type
            Me.Body = Body
        End Sub

        Private GeneratedFunction As CodeGen.ContextedFunction = Nothing
        Public Overrides Sub WriteSetterCall(Writer As CWriter, CompiledObject As String, NewValue As String)
            If GeneratedFunction Is Nothing Then
                GeneratedFunction = New CodeGen.ContextedFunction({$"{AssociatedType.cRepresentation} {INSTANCE_ARGUMENT_NAME}", $"{Type.cRepresentation} newValue"}, "void", Body, Type.ToString() & " SET " & Name)
                CodeGen.RegisterFunction(GeneratedFunction)
            End If
            Writer.WriteLine(GeneratedFunction.WriteCall({CompiledObject, NewValue}) & ";")
        End Sub
    End Class
End Namespace