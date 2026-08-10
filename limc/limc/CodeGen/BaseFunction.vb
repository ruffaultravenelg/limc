Imports System.IO

Namespace CodeGen
    Public Class BaseFunction

        'TODO compile using VERBOSE or not

        Protected Overridable ReadOnly Property Signature As String
        Protected Body As IEnumerable(Of String)
        Protected Comment As String

        Public Sub New(Signature As String, Body As IEnumerable(Of String), Optional Comment As String = "")
            Me.Signature = Signature
            Me.Body = Body
            Me.Comment = Comment
        End Sub

        Public Sub WriteSignature(Writer As StreamWriter)
            If VERBOSE AndAlso Not Comment = "" Then
                Writer.Write("/* ")
                Writer.Write(Comment)
                Writer.Write(" */ ")
            End If
            Writer.Write(Signature)
            Writer.WriteLine(";")
        End Sub
        Public Sub WriteBody(Writer As StreamWriter)

            If VERBOSE AndAlso Not Comment = "" Then
                Writer.Write("/* ")
                Writer.Write(Comment)
                Writer.WriteLine(" */")
            End If

            Writer.Write(Signature)
            Writer.WriteLine("{")
            For Each Line In Body
                Writer.WriteLine(vbTab & Line)
            Next
            Writer.WriteLine("}")
            Writer.WriteLine()

        End Sub

    End Class
End Namespace