'Represent a context of a construct that return something (use of the "return" statement)
Public Class ReturnableContext
    Inherits Context

    'Construcot
    Public Sub New(Parent As Context, Optional DefaultReturnType As Lim.Type = Nothing)
        MyBase.New(Parent)
        Me._ReturnType = DefaultReturnType
    End Sub

    'Return type
    Dim _ReturnType As Lim.Type
    Public ReadOnly Property ConstructReturnType As Lim.Type
        Get
            If _ReturnType Is Nothing Then
                Throw New ReturnTypeNotKnownYet()
            End If
            Return _ReturnType
        End Get
    End Property

    'Set return type
    Public Function CanIReturn(ValueType As Lim.Type) As Boolean

        'Value is already set => compare
        If _ReturnType IsNot Nothing Then
            Return _ReturnType = ValueType
        End If

        'Set value
        _ReturnType = ValueType
        Return True

    End Function

End Class
