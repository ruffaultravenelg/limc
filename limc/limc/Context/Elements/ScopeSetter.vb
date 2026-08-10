' Wrap a setter to provide "self" parameter automatically, represent a setter for a context in a struct/class
Public Class ScopeSetter

    Private Setter As Lazy.Setter
    Private UsePointerForInstance As Boolean

    Public ReadOnly Property Type As TypeSystem.Type
        Get
            Return Setter.Type
        End Get
    End Property

    Public Sub New(Setter As Lazy.Setter, UsePointerForInstance As Boolean)
        Me.Setter = Setter
        Me.UsePointerForInstance = UsePointerForInstance
    End Sub

    Public Sub CompileCall(Writer As CWriter, NewValue As String)
        If UsePointerForInstance Then
            Setter.WriteSetterCall(Writer, $"(&{Constants.INSTANCE_ARGUMENT_NAME})", NewValue)
        Else
            Setter.WriteSetterCall(Writer, Constants.INSTANCE_ARGUMENT_NAME, NewValue)
        End If
    End Sub

End Class
