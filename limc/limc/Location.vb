Public Structure Location
    Public File As Context.SourceFile

    Public FromLineNumber As Integer
    Public FromCol As Integer

    Public ToLineNumber As Integer
    Public ToCol As Integer

    '[FromCol; ToCol[

    Public Sub New(File As Context.SourceFile, FromLineNumber As Integer, FromCol As Integer, ToLineNumber As Integer, ToCol As Integer)
        Me.File = File
        Me.FromLineNumber = FromLineNumber
        Me.FromCol = FromCol
        Me.ToLineNumber = ToLineNumber
        Me.ToCol = ToCol
    End Sub
    Public Sub New(File As Context.SourceFile, FromLineNumber As Integer, FromCol As Integer, ToCol As Integer)
        Me.New(File, FromLineNumber, FromCol, FromLineNumber, ToCol)
    End Sub
    Public Sub New(Location As Location)
        Me.New(Location.File, Location.FromLineNumber, Location.FromCol, Location.ToLineNumber, Location.ToCol)
    End Sub

    'Addition
    Public Shared Operator +(a As Location, b As Location)
        If Not a.File.Filepath = b.File.Filepath Then
            Throw New Exception("Cannot join two location from two different files.")
        End If

        Return New Location(
            a.File,
            Math.Min(a.FromLineNumber, b.FromLineNumber),
            Math.Min(a.FromCol, b.FromCol),
            Math.Max(a.ToLineNumber, b.ToLineNumber),
            Math.Max(a.ToCol, b.ToCol)
        )
    End Operator

End Structure
