Imports System.Globalization
Imports System.Text

Namespace CodeGen
    Public Module Helper

        Public Function Sanitize(Value As String) As String
            ' Remove accents
            Dim normalized As String = Value.Normalize(NormalizationForm.FormD)
            Dim sb As New StringBuilder()

            For Each c As Char In normalized
                Dim uc As UnicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c)
                If uc <> UnicodeCategory.NonSpacingMark Then
                    sb.Append(c)
                End If
            Next

            Dim noAccents As String = sb.ToString().Normalize(NormalizationForm.FormC)

            ' Escape special characters
            ' IMPORTANT: Replace backslash FIRST
            Dim escaped As String = noAccents.Replace("\", "\\")

            ' Escape double quotes
            escaped = escaped.Replace("""", "\""")

            ' Escape control characters
            escaped = escaped.Replace(vbCr, "\r").Replace(vbLf, "\n").Replace(vbTab, "\t")

            Return escaped
        End Function

    End Module

End Namespace