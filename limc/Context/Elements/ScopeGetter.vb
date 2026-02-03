' Wrap a getter to provide "self" parameter automatically, represent a getter for a context in a struct/class
Public Class ScopeGetter

    Private Getter As Lazy.Getter

    Public ReadOnly Property Type As TypeSystem.Type
        Get
            Return Getter.Type
        End Get
    End Property

    Public Sub New(Getter As Lazy.Getter)
        Me.Getter = Getter
    End Sub

    Public Function CompileCall() As String
        Return Me.Getter.CallGetter(Constants.INSTANCE_ARGUMENT_NAME)
    End Function

End Class
