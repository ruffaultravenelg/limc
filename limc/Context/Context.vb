Public Class Context

    Public ReadOnly Property Parent As Context

    Public Sub New(Optional Parent As Context = Nothing)
        Me.Parent = Parent
    End Sub

    Public ReadOnly Iterator Property AllParents As IEnumerable(Of Context)
        Get
            Dim Current As Context = Me
            While Current IsNot Nothing
                Yield Current
                Current = Current.Parent
            End While
        End Get
    End Property

    Public Function HasParent(Of T As Context)() As Boolean
        For Each Ctx As Context In AllParents
            If TypeOf Ctx Is T Then
                Return True
            End If
        Next
        Return False
    End Function
    Public Function GetParent(Of T As Context)() As T
        For Each Ctx As Context In AllParents
            If TypeOf Ctx Is T Then
                Return Ctx
            End If
        Next
        Return Nothing
    End Function

End Class
