Namespace C

    Public Class [Enum]

        'Properties
        Private Name As String
        Private Values As IEnumerable(Of String)

        'Constructor
        Public Sub New(Name As String, Values As IEnumerable(Of String))
            Me.Name = Name
            Me.Values = Values
        End Sub

        'Write
        Public Sub WriteTypedef(Stream As IO.StreamWriter)
            Stream.Write("typedef enum {")
            Stream.Write(String.Join(", ", Values))
            Stream.Write("} ")
            Stream.Write(Name)
            Stream.WriteLine(";")
        End Sub

    End Class

End Namespace