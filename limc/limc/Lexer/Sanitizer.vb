Namespace Lexer
    Friend Class Sanitizer

        '
        ' Remove multiples linestart ([linestart][linestart]) -> ([linestart])
        '
        Public Shared Sub SanitizeTokenList(Tokens As List(Of Token))

            Dim LastWasLinestart As Boolean = False
            Dim i = 0

            While (i < Tokens.Count)

                Dim Tok = Tokens(i)

                If Tok.Type = TokenType.LINESTART Then
                    If LastWasLinestart Then
                        Tokens.RemoveAt(i - 1)
                    Else
                        LastWasLinestart = True
                        i += 1
                    End If
                Else
                    LastWasLinestart = False
                    i += 1
                End If

            End While

        End Sub

    End Class
End Namespace