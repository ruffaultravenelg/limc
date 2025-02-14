Namespace Lim

    '
    ' Contains all the getters of a type
    '
    Public Class GetterComponent

        ' Getters
        Private Getters As New Dictionary(Of String, GetterInvoker)

        ' Constructor
        Public Sub New()

        End Sub

        'Compile a call to a getter
        Public Function CallGetter(Name As String, Obj As String) As String
            Return Getters(Name).CompileCall(Obj)
        End Function

        'Do getter exist
        Public Function HasGetter(Name As String) As Boolean
            Return Getters.ContainsKey(Name)
        End Function

        'Get getter type
        Public Function GetGetterType(Name As String) As Lim.Type
            Return Getters(Name).Type
        End Function

        'Register a new getter
        Public Sub RegisterGetter(Name As String, GetterInvoker As GetterInvoker)
            Getters.Add(Name, GetterInvoker)
        End Sub

    End Class

End Namespace