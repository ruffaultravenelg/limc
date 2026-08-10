Imports System.IO

Namespace CodeGen
    Public Class Union

        Private Name As String
        Private Fields As IEnumerable(Of String)
        Private Comment As String = ""

        Public Sub New(Name As String, Fields As IEnumerable(Of String), Optional Comment As String = "")
            Me.Name = Name
            Me.Fields = Fields
            Me.Comment = Comment
        End Sub

        Public Sub WriteSignature(Writer As StreamWriter)
            Writer.Write("typedef union ")
            Writer.Write(Name)
            Writer.Write(" ")
            Writer.Write(Name)
            If VERBOSE AndAlso Not Comment = "" Then
                Writer.Write("; //")
                Writer.WriteLine(Comment)
            Else
                Writer.WriteLine(";")
            End If
        End Sub

        Public Sub Write(Writer As StreamWriter)
            If VERBOSE AndAlso Not Comment = "" Then
                Writer.WriteLine("// " & Comment)
            End If
            Writer.Write("typedef union ")
            Writer.Write(Name)
            Writer.WriteLine("{")
            For Each Field As String In Fields
                Writer.WriteLine(vbTab & TypeSystem.RackType.ValidateStructFieldDefinition(Field))
            Next
            Writer.Write("} ")
            Writer.Write(Name)
            Writer.WriteLine(";")
            Writer.WriteLine()
        End Sub

    End Class
End Namespace