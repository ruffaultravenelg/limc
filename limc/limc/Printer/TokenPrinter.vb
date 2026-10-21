Imports limc.Lexer
Imports limc.OutputStream

Namespace Printer
    Friend Class TokenPrinter

        Public Shared Sub PrintTokens(Writer As IOutputStream, Tokens As IReadOnlyList(Of Token))
            Writer.Writeline(String.Join(" "c, Tokens))
        End Sub

    End Class
End Namespace