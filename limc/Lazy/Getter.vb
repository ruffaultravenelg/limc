Namespace Lazy
    Public MustInherit Class Getter

        Public MustOverride ReadOnly Property Name As String
        Public MustOverride ReadOnly Property Type As TypeSystem.Type
        Public MustOverride Function CallGetter(CompiledObject As String) As String

    End Class
End Namespace