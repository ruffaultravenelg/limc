Public Class Scope
    Inherits Context

    Private Lines As New List(Of String)
    Public ReadOnly Property Location As Location

    Public Sub New(Parent As Context, Location As Location)
        MyBase.New(Parent)
        Me.Location = Location
    End Sub

    Public Sub WriteLine(Line As String)
        Lines.Add(Line)
    End Sub

    Public Sub WriteScope(Scope As Scope)
        For Each Line In Scope.Lines
            Lines.Add(vbTab & Line)
        Next
    End Sub

    Public Function GetLines() As IEnumerable(Of String)
        Return Lines
    End Function

End Class
