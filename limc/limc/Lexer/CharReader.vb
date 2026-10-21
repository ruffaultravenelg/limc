Imports System.IO
Namespace Lexer
    '
    ' Wrapper for TextReader to add
    ' - a trailing caracter to simplify the tokenizer
    ' - peak the next char in the stream
    ' - unifying newline characters
    '
    Friend Class CharReader
        Private Const TrailingChar As Char = vbLf

        Private ReadOnly _reader As TextReader  '-1 = EOF, ChrW(Reader.Read()) = Char
        Private _trailingCharInjected As Boolean = False

        Public ReadOnly Property Filepath As String
        Public ReadOnly Property LineIndex As Integer
        Public ReadOnly Property ColIndex As Integer
        Public ReadOnly Property Current As Char
        Public ReadOnly Property Peek As Char
        Public ReadOnly Property EndOfFile As Boolean

        Public Sub New(reader As TextReader, filepath As String)
            If reader Is Nothing Then Throw New ArgumentNullException(NameOf(reader))

            _reader = reader
            Me.Filepath = filepath

            ' Set pos for the first char
            _LineIndex = 0
            _ColIndex = -1 ' Will be set to 0 with the first Advance()

            ' Amorce la fenêtre glissante
            Advance()
            Advance()
        End Sub

        Public Sub Advance()
            If EndOfFile Then
                Throw New UnexpectedEndOfFile($"Unexpected end of file in ""{Filepath}"".")
            End If

            ' Updating char
            If _Current = vbLf Then
                _LineIndex += 1
                _ColIndex = 0
            ElseIf _Current <> ChrW(0) Then
                _ColIndex += 1
            Else
                ' first start
                _ColIndex = 0
            End If

            ' <-
            _Current = _Peek

            ' Next char
            Dim raw As Integer = _reader.Read()

            If raw <> -1 Then
                Dim c As Char = ChrW(raw)

                ' Handling newline
                If c = vbCr Then
                    ' If windows, just ignore \r
                    If _reader.Peek() = AscW(vbLf) Then
                        _reader.Read()
                    End If
                    _Peek = vbLf 'Alwas use \n
                Else
                    _Peek = c
                End If

            ElseIf Not _trailingCharInjected Then
                _Peek = TrailingChar 'Att last newline
                _trailingCharInjected = True

            Else
                _Peek = ChrW(0) 'no more
                _EndOfFile = True
            End If
        End Sub

    End Class

End Namespace