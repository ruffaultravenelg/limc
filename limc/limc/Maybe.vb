Friend Class Maybe(Of T)

    Private ReadOnly _Value As T
    Private Sub New(Value As T)
        Me._Value = Value
    End Sub

    Public ReadOnly Property HasValue As Boolean
        Get
            Return _Value IsNot Nothing
        End Get
    End Property
    Public ReadOnly Property IsEmpty As Boolean
        Get
            Return _Value Is Nothing
        End Get
    End Property

    Public ReadOnly Property Value As T
        Get
            If _Value Is Nothing Then
                Throw New InternalException("Access maybe value without a value")
            Else
                Return _Value
            End If
        End Get
    End Property


    ' ==== SHARED ====

    Public Shared Function Some(Value As T) As Maybe(Of T)
        Return New Maybe(Of T)(Value)
    End Function

    Public Shared Function None() As Maybe(Of T)
        Return New Maybe(Of T)(Nothing)
    End Function

End Class
