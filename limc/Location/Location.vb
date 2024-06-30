Public Class Location

    Public ReadOnly File As LimSource

    Public ReadOnly FromLineNumber As Integer
    Public ReadOnly FromCharIndex As Integer

    Public ReadOnly ToLineNumber As Integer
    Public ReadOnly ToCharIndex As Integer

    Public Sub New(File As LimSource, FromLineNumber As Integer, FromCharIndex As Integer, ToLineNumber As Integer, ToCharIndex As Integer)
        Me.File = File
        Me.FromLineNumber = FromLineNumber
        Me.FromCharIndex = FromCharIndex
        Me.ToLineNumber = ToLineNumber
        Me.ToCharIndex = ToCharIndex
    End Sub

    Public Shared Operator &(A As Location, B As Location) As Location
        Return New Location(A.File, Math.Min(A.FromLineNumber, B.FromLineNumber), Math.Min(A.FromCharIndex, B.FromCharIndex), Math.Max(A.ToLineNumber, B.ToLineNumber), Math.Max(A.ToCharIndex, B.ToCharIndex))
    End Operator

End Class
