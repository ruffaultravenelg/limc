'Represent the context for statements and expressions, but with lines (a scope)
Public Class Scope
    Inherits Context

    'Lines
    Private Lines As New List(Of String)

    'New
    Public Sub New(Optional Parent As Context = Nothing)
        MyBase.New(Parent)
    End Sub

    'Build
    Public Function Build() As IEnumerable(Of String)
        Return Lines
    End Function
    Public Iterator Function BuildWithTabs() As IEnumerable(Of String)
        For Each Line As String In Lines
            Yield vbTab & Line
        Next
    End Function

End Class
