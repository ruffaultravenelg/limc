Public Class IteratorAdapter(Of T As ILocated)

    Private Index As Integer = 0
    Private Source As IEnumerable(Of T)

    Public Sub New(Source As IEnumerable(Of T))
        If Source Is Nothing Then
            Throw New ArgumentNullException(NameOf(Source))
        End If
        Me.Source = Source
        Index = -1
        [Next]()
    End Sub

    Public ReadOnly Property Last As T
        Get
            Return Source(Index - 1)
        End Get
    End Property

    Private _Current As T
    Public ReadOnly Property Current As T
        Get
            Return _Current
        End Get
    End Property

    Public ReadOnly Property HasNext As Boolean
        Get
            Return Index + 1 < Source.Count
        End Get
    End Property

    Public Sub [Next]()

        If Not HasNext Then
            Throw New SyntaxException("A element was expected after this token", Current.Location)
        End If

        Index += 1
        _Current = Source(Index)

    End Sub

    Public Function SaveState() As Integer
        Return Index
    End Function
    Public Sub LoadState(State As Integer)
        Me.Index = State - 1
        [Next]()
    End Sub

End Class