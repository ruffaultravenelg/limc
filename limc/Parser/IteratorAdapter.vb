Public Class IteratorAdapter(Of T As ILocated)
    Private ReadOnly Enumerator As IEnumerator(Of T)

    Private _HasNext As Boolean
    Private LastValue As T
    Private LastLastValue As T

    Public Sub New(source As IEnumerable(Of T))
        If source Is Nothing Then
            Throw New ArgumentNullException(NameOf(source))
        End If
        Enumerator = source.GetEnumerator()

        Enumerator.MoveNext()
        LastLastValue = Nothing
        LastValue = Enumerator.Current
        _HasNext = Enumerator.MoveNext()

    End Sub

    Public ReadOnly Property Last As T
        Get
            Return LastLastValue
        End Get
    End Property

    Public ReadOnly Property Current As T
        Get
            Return LastValue
        End Get
    End Property

    Public ReadOnly Property HasNext As Boolean
        Get
            Return _HasNext
        End Get
    End Property

    Public Function [Next]() As T

        If Not HasNext Then
            Throw New SyntaxException("A element was expected after this token", Current.Location)
        End If

        LastLastValue = LastValue
        LastValue = Enumerator.Current
        _HasNext = Enumerator.MoveNext()

        Return LastValue

    End Function

End Class