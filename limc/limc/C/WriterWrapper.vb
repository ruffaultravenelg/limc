Imports System.IO

Namespace C
    Friend Class WriterWrapper

        Private fs As StreamWriter
        Private indentationLevel As Integer

        Public Sub New(fs As StreamWriter)
            Me.fs = fs
            indentationLevel = 0
        End Sub

        ' Indentation level
        Public Sub IncreaseIndentation()
            indentationLevel += 1
        End Sub
        Public Sub DecreaseIndentation()
            indentationLevel -= 1
            If indentationLevel < 0 Then
                Throw New InternalException("Indentation too low in HIR Compiler")
            End If
        End Sub

        ' Write with indentation level
        Public Sub Startline(Optional Line As String = "")
            fs.Write(System.Environment.NewLine)
            fs.Write(Strings.StrDup(indentationLevel, vbTab))
            fs.Write(Line)
        End Sub

        ' Append string
        Public Sub Write(Text As String)
            fs.Write(Text)
        End Sub


    End Class
End Namespace