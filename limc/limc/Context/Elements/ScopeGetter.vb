' Wrap a getter to provide "self" parameter automatically, represent a getter for a context in a struct/class
Public Class ScopeGetter

    Private Getter As Lazy.Getter
    Private UsePointerForInstance As Boolean

    Public ReadOnly Property Type As TypeSystem.Type
        Get
            Return Getter.Type
        End Get
    End Property

    Public Sub New(Getter As Lazy.Getter, UsePointerForInstance As Boolean)
        Me.Getter = Getter
        Me.UsePointerForInstance = UsePointerForInstance
    End Sub

    Public Function CompileCall() As String
        Return Me.Getter.CallGetter(Constants.INSTANCE_ARGUMENT_NAME)
    End Function

    Public Function CallGetterButReturnsValuePointer(Location As Location) As String
        If UsePointerForInstance Then
            Return Me.Getter.CallGetterButReturnsValuePointer($"(&{Constants.INSTANCE_ARGUMENT_NAME})", Location)
        Else
            Return Me.Getter.CallGetterButReturnsValuePointer(Constants.INSTANCE_ARGUMENT_NAME, Location)
        End If
    End Function


End Class
