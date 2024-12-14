Imports System.Text

Public Class FileAST

    Public ReadOnly Import As New List(Of Source.Import)
    Public ReadOnly Functions As New List(Of Source.Function)

    'To string
    Public Overrides Function ToString() As String

        'Add each properties
        Dim Objs As New List(Of Object)
        Objs.AddRange(Import)
        Objs.AddRange(Functions)

        'Create string builder
        Dim Result As New StringBuilder()

        'Print each element to string
        For Each Obj In Objs
            Result.Append(Obj.ToString())
            Result.Append(Environment.NewLine)
        Next

        'Return result
        Return Result.ToString()

    End Function

End Class
