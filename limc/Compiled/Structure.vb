Namespace C

    Public Class [Structure]

        'Properties
        Private Name As String
        Private Fields As IEnumerable(Of String)

        'Constructor
        Public Sub New(Name As String, Fields As IEnumerable(Of String))
            Me.Name = Name
            Me.Fields = Fields
        End Sub

        'Write signature
        '   typedef struct type_t type_t;
        Public Sub WriteTypedefSignature(Stream As IO.StreamWriter)
            Stream.Write("typedef struct ")
            Stream.Write(Name)
            Stream.Write(" ")
            Stream.Write(Name)
            Stream.WriteLine(";")
        End Sub

        'Write definition
        '   typedef struct type_t {
        '       ...
        '   } type_t;
        Public Sub WriteDefinition(Stream As IO.StreamWriter)
            Stream.Write("typedef struct ")
            Stream.Write(Name)
            Stream.WriteLine("{")
            For Each Field As String In Fields
                Stream.Write(vbTab)
                Stream.Write(Field)
                Stream.WriteLine(";")
            Next
            Stream.Write("} ")
            Stream.Write(Name)
            Stream.WriteLine(";")
        End Sub

    End Class

End Namespace