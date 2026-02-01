Namespace Lazy
    Public Class HardCalculatedGetter
        Inherits Getter

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

        Private GeneratedFunction As CodeGen.PassingContextFunction = Nothing
        Public Overrides Function CallGetter(CompiledObject As String) As String
            If GeneratedFunction Is Nothing Then
                GeneratedFunction = New CodeGen.PassingContextFunction({$"{AssociatedType.cRepresentation} {INSTANCE_ARGUMENT_NAME}"}, Type.cRepresentation, Body, Type.ToString() & " GET " & Name)
                CodeGen.RegisterFunction(GeneratedFunction)
            End If
            Return GeneratedFunction.WriteCall({CompiledObject})
        End Function

    End Class
End Namespace