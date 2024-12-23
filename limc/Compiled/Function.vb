Namespace C

    Public Class [Function]

        'Properties
        Private Signature As String
        Private Body As IEnumerable(Of String)

        'Constructor
        Public Sub New(Signature As String, Body As IEnumerable(Of String))
            Me.Signature = Signature
            Me.Body = Body
        End Sub

        'Write signature
        Public Sub WriteSignature(Stream As IO.StreamWriter)
            Stream.WriteLine(Signature & ";")
        End Sub

        'Write boyd
        Public Sub WriteBody(Stream As IO.StreamWriter)
            Stream.Write(Signature)
            Stream.WriteLine("{")
            Stream.WriteLine()
            For Each Line As String In Body
                Stream.WriteLine(vbTab & Line)
            Next
            Stream.WriteLine()
            Stream.WriteLine("}")
        End Sub

    End Class

End Namespace