Imports System.IO

Module LineParser

    '
    ' Constants
    '
    Private Const LINE_COMMENT As String = "//"

    '
    ' Read a file and parse each line into a LimSourceLine
    '
    Public Function Parse(filepath As String) As List(Of LimSourceLine)

        'Create result
        Dim result As New List(Of LimSourceLine)

        'Some useful variables
        Dim TabContent As String = "" 'What string is used to define a indentation
        Dim LinePosition As Integer = 0 'Line index

        'Open the file
        Dim reader As New StreamReader(filepath)

        'Read each line
        Do Until reader.EndOfStream

            'Get line
            Dim line As String = reader.ReadLine()
            Dim lineIndentation As Integer = 0
            LinePosition += 1

            'Get indentation
            If TabContent = "" Then

                'Indentation content is not defined
                While (line.Length > 0 AndAlso Char.IsWhiteSpace(line(0)))
                    TabContent &= line(0)
                    line = line.Substring(1)
                End While
                If TabContent.Length > 0 Then
                    lineIndentation = 1
                End If

            Else

                'Check indentation
                While (line.StartsWith(TabContent))
                    lineIndentation += 1
                    line = line.Substring(TabContent.Length)
                End While

            End If

            'Comment
            If line.StartsWith(LINE_COMMENT) Then
                Continue Do
            End If

            'Empty line
            If line = "" Then
                Continue Do
            End If

            'Normal line content
            result.Add(New LimSourceLine(LinePosition, lineIndentation * TabContent.Length, lineIndentation, line))

        Loop

        'Close file
        reader.Close()

        'Return result
        Return result

    End Function

End Module
