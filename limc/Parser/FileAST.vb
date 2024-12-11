Imports System.Text

Public Class FileAST

    Public ReadOnly Import As New List(Of Source.Import)
    Public ReadOnly Functions As New List(Of Source.Function)

    'To string
    Public Overrides Function ToString() As String

        'Create string builder
        Dim Result As New StringBuilder()

        'Add imports
        For Each Im In Import
            Result.Append(Im.ToString())
            Result.Append(Environment.NewLine)
        Next

        'Return result
        Return Result.ToString()

    End Function

End Class
