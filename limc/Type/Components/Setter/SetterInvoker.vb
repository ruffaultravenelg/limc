Namespace Lim

    '
    ' Represet the way to compile a assignation to a specified setter, the rest is handled by the SetterComponent class
    '
    Public MustInherit Class SetterInvoker

        'The type of value need for the setter
        Public Overridable ReadOnly Property Type As Lim.Type

        'Compile variable assignation
        Public MustOverride Sub CompileAssignation(Scope As Scope, Obj As String, NewValue As String)

        'Constructor
        Public Sub New(NeededType As Type)
            Me.Type = NeededType
        End Sub

    End Class

End Namespace