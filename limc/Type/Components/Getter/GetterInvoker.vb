Namespace Lim

    '
    ' Represet the way to compile a call to a specified getter, the rest is handled by the GetterComponent class
    '
    Public MustInherit Class GetterInvoker

        'The type of value returned by the getter
        Public Overridable ReadOnly Property Type As Lim.Type

        'Compile call
        Public MustOverride Function CompileCall(Obj As String) As String

        'Constructor
        Public Sub New(ReturnedType As Type)
            Me.Type = ReturnedType
        End Sub

    End Class

End Namespace