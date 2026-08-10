Namespace Lazy
    Public Class DirectAccessSetter
        Inherits Setter

        Public Overrides ReadOnly Property Name As String
        Public Overrides ReadOnly Property Type As TypeSystem.Type

        Private Callback As Func(Of String, String, String)

        Public Sub New(Name As String, Type As TypeSystem.Type, Callback As Func(Of String, String, String))
            Me.Name = Name
            Me.Type = Type
            Me.Callback = Callback
        End Sub

        Public Overrides Sub WriteSetterCall(Writer As CWriter, CompiledObject As String, NewValue As String)
            Writer.WriteLine(Callback(CompiledObject, NewValue))
        End Sub

    End Class
End Namespace