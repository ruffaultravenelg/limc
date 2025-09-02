Imports System.IO

Namespace CodeGen
    Public Class [Function]

        'TODO compile using VERBOSE or not

        Private Signature As String
        Private Body As IEnumerable(Of String)

        Public Sub New(Signature As String, Body As IEnumerable(Of String))
            Me.Signature = Signature
            Me.Body = Body
        End Sub

        Public Sub WriteSignature(Writer As StreamWriter)
            Writer.Write(Signature)
            Writer.WriteLine(";")
        End Sub
        Public Sub WriteBody(Writer As StreamWriter)

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