' Wrap a setter to provide "self" parameter automatically, represent a setter for a context in a struct/class
Public Class ScopeSetter

    Private Setter As Lazy.Setter

    Public ReadOnly Property Type As TypeSystem.Type
        Get
            Return Setter.Type
        End Get
    End Property

    Public Sub New(Setter As Lazy.Setter)
        Me.Setter = Setter
    End Sub

    Public Sub CompileCall(Writer As CWriter, NewValue As String)
        Setter.WriteSetterCall(Writer, Constants.INSTANCE_ARGUMENT_NAME, NewValue)
    End Sub

End Class
