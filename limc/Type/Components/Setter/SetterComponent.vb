Namespace Lim

    '
    ' Contains all the setters of a type
    '
    Public Class SetterComponent

        ' Getters
        Private Setters As New Dictionary(Of String, SetterInvoker)

        'Compile a setter call
        Public Sub CompileAssignation(Name As String, Scope As Scope, Obj As String, NewValue As String)
            Setters(Name).CompileAssignation(Scope, Obj, NewValue)
        End Sub

        'Do setter exist
        Public Function HasSetter(Name As String) As Boolean
            Return Setters.ContainsKey(Name)
        End Function

        'Get setter type
        Public Function GetSetterType(Name As String) As Lim.Type
            Return Setters(Name).Type
        End Function

        'Register a new setter
        Public Sub RegisterSetter(Name As String, GetterInvoker As SetterInvoker)
            Setters.Add(Name, GetterInvoker)
        End Sub

    End Class

End Namespace