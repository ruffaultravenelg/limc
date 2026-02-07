Namespace Lazy
    Public MustInherit Class Setter

        Public MustOverride ReadOnly Property Name As String
        Public MustOverride ReadOnly Property Type As TypeSystem.Type
        Public MustOverride Sub WriteSetterCall(Writer As CWriter, CompiledObjectPointer As String, NewValue As String)

    End Class
End Namespace