'
' Contains a location in file
' Can be :
' - just a file
' - a line interval (from line x to line y)
' - a expression location (line x col y length z)
' 
' Cannot use interfaces due to boxing & class due to too many allocs
'
Imports System.IO

Friend Structure Location

    Public ReadOnly Property Filename As String

    Private StartLineIndex As Integer
    Private EndLineIndex As Integer

    Private ColumnIndex As Integer
    Private ExprLength As Integer

    Private ReadOnly Property Kind As LocationKind
        Get
            If StartLineIndex < 0 Then Return LocationKind.FileOnly
            If ExprLength < 0 Then Return LocationKind.Multiline
            Return LocationKind.Expression
        End Get
    End Property

    ' Constructor Kind=FileOnly
    Public Sub New(Filename As String)
        Me.Filename = Filename
        Me.StartLineIndex = -1
        Me.EndLineIndex = -1
        Me.ColumnIndex = -1
        Me.ExprLength = -1
    End Sub

    ' Constructor Kind=Multiline
    Public Sub New(Filename As String, StartLineIndex As Integer, EndLineIndex As Integer)
        Me.Filename = Filename
        Me.StartLineIndex = StartLineIndex
        Me.EndLineIndex = EndLineIndex
        Me.ColumnIndex = -1
        Me.ExprLength = -1
    End Sub

    ' Constructor Kind=Expression
    Public Sub New(Filename As String, LineIndex As Integer, ColumnIndex As Integer, Length As Integer)
        Me.Filename = Filename
        Me.StartLineIndex = LineIndex
        Me.ColumnIndex = ColumnIndex
        Me.ExprLength = Length
        Me.EndLineIndex = -1
    End Sub

    '
    ' Merge two Location in a new one that include both
    '
    Public Shared Operator +(a As Location, b As Location) As Location

        If a.Filename <> b.Filename Then
            Throw New InternalException("Cannot add two location with different files")
        End If

        If a.Kind = LocationKind.FileOnly OrElse b.Kind = LocationKind.FileOnly Then
            Return New Location(a.Filename)
        End If

        ' Only two expr can add up to make a new expr location
        If a.Kind = LocationKind.Expression AndAlso b.Kind = LocationKind.Expression AndAlso a.StartLineIndex = b.StartLineIndex Then

            ' Assert arguments
            If a.StartLineIndex <> b.StartLineIndex Then
                Throw New InternalException("Cannot add two Location with different line index")
            End If

            ' Ensure a col starts before b
            If b.ColumnIndex < a.ColumnIndex Then
                Dim tmp = a
                a = b
                b = tmp
            End If

            ' Calculate length
            Dim NewLength = Math.Max(
                a.ExprLength,
                (b.ColumnIndex - a.ColumnIndex) + b.ExprLength
            )

            ' Create new Location
            Return New Location(a.Filename, a.StartLineIndex, a.ColumnIndex, NewLength)

        End If

        ' Adding to multiline
        Dim startLine = Math.Min(a.StartLineIndex, b.StartLineIndex)
        Dim endLine = Math.Max(Math.Max(a.EndLineIndex, 0), Math.Max(b.EndLineIndex, 0))
        Return New Location(a.Filename, startLine, endLine)

    End Operator

    ' Get lines to show for exception rendering
    Public Function GetLines() As IEnumerable(Of String)
        Select Case Kind
            Case LocationKind.FileOnly
                Return {}
            Case LocationKind.Multiline
                Return GetMultilineLines()
            Case LocationKind.Expression
                Return GetExprLines()
            Case Else
                Throw New InternalException("Unknown location type")
        End Select
    End Function

    ' Read from the file and format the code sample
    ' Returns a List(Of String):
    '
    ' 45 | let variable = test[5]
    '                     ^^^^
    Private Function GetExprLines() As IEnumerable(Of String)

        ' Create line prefiex ("42 | ")
        Dim LinePrefix As String = (StartLineIndex + 1).ToString() & " | "

        ' Read line from the file
        Dim Line As String = LinePrefix
        Using Reader As New StreamReader(Filename)
            For i = 0 To StartLineIndex - 1
                Reader.ReadLine()
            Next
            Line &= Reader.ReadLine().Replace(vbTab, " ") ' Replace tab by space for underline consistency
        End Using

        ' Create underline
        Dim Underline As String = StrDup(LinePrefix.Length + ColumnIndex, " "c) & StrDup(ExprLength, "^"c)

        ' Return list
        Return {Line, Underline}

    End Function

    ' Read from the file and format the code sample
    ' Returns a List(Of String):
    '
    ' 45 | let variable = 4
    ' 46 | puts(variable.str())
    Private Function GetMultilineLines() As IEnumerable(Of String)

        ' Read line from the file
        Dim Lines As New List(Of String)
        Using Reader As New StreamReader(Filename)
            For i = 0 To StartLineIndex - 1
                Reader.ReadLine()
            Next
            For y = StartLineIndex To EndLineIndex
                Lines.Add(y & " | " & Reader.ReadLine().Replace(vbTab, " ")) ' Replace tab by space for underline consistency
            Next
        End Using

        ' Return list
        Return Lines

    End Function

    ' To string for exception rendering
    Public Overrides Function ToString() As String
        Select Case Kind
            Case LocationKind.FileOnly
                Return $"File <{Filename}>"
            Case LocationKind.Multiline
                Return $"From line {StartLineIndex + 1} to line {EndLineIndex + 1} of file <{Filename}>"
            Case LocationKind.Expression
                Return $"Line {StartLineIndex + 1} col {ColumnIndex + 1} of file <{Filename}>"
            Case Else
                Throw New InternalException("Unknown location type")
        End Select
    End Function

    Private Enum LocationKind
        FileOnly
        Expression
        Multiline
    End Enum

End Structure