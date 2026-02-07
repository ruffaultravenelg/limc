Namespace Lazy
    Public MustInherit Class Getter

        Public MustOverride ReadOnly Property Name As String
        Public MustOverride ReadOnly Property Type As TypeSystem.Type
        Public MustOverride Function CallGetter(CompiledObject As String) As String
        Protected Overridable Function _GetReference(CompiledObjectPointer As String, Location As Location) As String
            Throw New ExpressionDoesNotReferToAVariableError(Location)
        End Function

        Public Overridable Function GetReference(CompiledObjectPointer As String, Location As Location) As String
            If Type.IsPointer Then
                Return CallGetter(CompiledObjectPointer)
            Else
                Return _GetReference(CompiledObjectPointer, Location)
            End If
        End Function

    End Class
End Namespace