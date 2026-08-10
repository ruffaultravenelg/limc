Imports System.IO

Namespace CodeGen
    Public Class [Enum]

        Private Name As String
        Private Options As IEnumerable(Of String)
        Private Comment As String

        Public Sub New(Name As String, Options As IEnumerable(Of String), Optional Comment As String = "")
            Me.Name = Name
            Me.Options = Options
            Me.Comment = Comment
        End Sub

        Public Sub Write(Writer As StreamWriter)
            If Not String.IsNullOrEmpty(Comment) Then
                Writer.Write("/* ")
                Writer.Write(Comment)
                Writer.Write(" */ ")
            End If
            Writer.Write("typedef enum {")
            For i As Integer = 0 To Options.Count() - 1
                If Not i = 0 Then
                    Writer.Write(", ")
                End If
                Writer.Write(Options(i))
            Next
            Writer.Write("} ")
            Writer.Write(Name)
            Writer.WriteLine(";")
        End Sub

    End Class
End Namespace