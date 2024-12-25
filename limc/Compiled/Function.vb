Namespace C

    Public Class [Function]

        'Properties
        Private Signature As String
        Private Body As IEnumerable(Of String)

        'Contexted function count
        Private Shared Contexted As New List(Of String)

        'Constructor
        Public Sub New(Signature As String, Body As IEnumerable(Of String))
            Me.Signature = Signature
            Me.Body = Body
        End Sub
        Public Sub New(Name As String, Arguments As IEnumerable(Of String), Returntype As String, Body As IEnumerable(Of String))

            'Create signature
            Signature = Returntype & " " & Name & "("
            Signature &= CONTEXT_STRUCTURENAME & "* parent_" & CONTEXT_NAME
            For Each Arg As String In Arguments
                Signature &= ", " & Arg
            Next
            Signature &= ")"

            'Create body
            Dim NewBody As New List(Of String)
            NewBody.Add("")
            NewBody.Add("INIT_" & CONTEXT_NAME.ToUpper() & "(parent_" & CONTEXT_NAME & ", " & Contexted.Count.ToString() & ");")
            NewBody.AddRange(Body)
            Me.Body = NewBody

            'Add name to contexted
            Contexted.Add(Name)

        End Sub

        'Write signature
        Public Sub WriteSignature(Stream As IO.StreamWriter)
            Stream.WriteLine(Signature & ";")
        End Sub

        'Write body
        Public Sub WriteBody(Stream As IO.StreamWriter)
            Stream.Write(Signature)
            Stream.WriteLine("{")
            For Each Line As String In Body
                Stream.WriteLine(vbTab & Line)
            Next
            Stream.WriteLine()
            Stream.WriteLine("}")
        End Sub

        'Write function map
        Public Shared Sub WriteFunctionMap(Stream As IO.StreamWriter)
            Stream.WriteLine("fn_entry fn_name_map[] = {")
            For i As Integer = 0 To Contexted.Count - 1
                Stream.Write(vbTab)
                Stream.Write("{")
                Stream.Write(i.ToString())
                Stream.Write(", """)
                Stream.Write(Contexted(i))
                Stream.WriteLine("""},")
            Next
            Stream.WriteLine("};")
        End Sub

    End Class

End Namespace