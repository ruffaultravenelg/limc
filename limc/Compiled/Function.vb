Namespace C

    Public Class [Function]

        'Properties
        Private Signature As String
        Private Body As IEnumerable(Of String)
        Private Description As String

        'Contexted function count
        Private Shared Contexted As New List(Of String) From {"[Thread Start]"}

        'Constructor
        Public Sub New(Signature As String, Body As IEnumerable(Of String), Optional Description As String = "")
            Me.Signature = Signature
            Me.Body = Body
            Me.Description = Description
        End Sub
        Public Sub New(Name As String, Arguments As IEnumerable(Of String), Returntype As String, Body As IEnumerable(Of String), Optional Description As String = "")

            'Create signature
            Signature = Returntype & " " & Name & "("
            Signature &= CONTEXT_STRUCTURENAME & "* " & CONTEXT_PARENT
            For Each Arg As String In Arguments
                Signature &= ", " & Arg
            Next
            Signature &= ")"

            'Create body
            Dim NewBody As New List(Of String)
            'NewBody.Add("")
            NewBody.Add("INIT_" & CONTEXT_NAME.ToUpper() & "(" & CONTEXT_PARENT & ", " & Contexted.Count.ToString() & ");")
            NewBody.AddRange(Body)
            Me.Body = NewBody

            'Add name to contexted
            Contexted.Add(Name)

            'Set description
            Me.Description = Description

        End Sub

        'Write signature
        Public Sub WriteSignature(Stream As IO.StreamWriter)
            If Description.Length > 0 AndAlso ArgumentHandler.Comment_Sources Then
                Stream.Write("/* " & Description & " */ ")
            Else
            End If
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

        'Write function call
        Public Shared Function WriteCall(FunctionName As String, ParamArray Arguments() As String) As String
            Dim finalArgs As String = "&" & CONTEXT_NAME

            For Each arg As String In Arguments
                If String.IsNullOrWhiteSpace(arg) Then Continue For

                ' Supprimer les espaces de début
                arg = arg.TrimStart()

                ' Ajouter une virgule si nécessaire
                If finalArgs.Length > 0 AndAlso Not finalArgs.EndsWith(", ") AndAlso Not arg.StartsWith(",") Then
                    finalArgs &= ", "
                ElseIf finalArgs.Length > 0 AndAlso Not finalArgs.EndsWith(" ") AndAlso arg.StartsWith(",") Then
                    finalArgs &= " "
                End If

                finalArgs &= arg
            Next

            Return $"{FunctionName}({finalArgs})"
        End Function

    End Class

End Namespace