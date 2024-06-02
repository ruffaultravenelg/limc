Imports System.IO
Imports System.Threading.Tasks.Dataflow

Module LineParser

    '
    ' Constants
    '
    Private Const LINE_COMMENT As String = "//"
    Private Const CONDITIONNAL_LINE_IF As String = "$IF "
    Private Const CONDITIONNAL_LINE_IFNOT As String = "$IFNOT "
    Private Const CONDITIONNAL_LINE_END As String = "$END"

    '
    ' Read a file and parse each line into a LimSourceLine
    '
    Public Function Parse(filepath As String) As List(Of LimSourceLine)

        'Create result
        Dim result As New List(Of LimSourceLine)

        'Some useful variables
        Dim TabContent As String = "" 'What string is used to define a indentation
        Dim SearchingForEnd As Integer = -1 'continue until $END and it's equal to 0
        Dim ExceptEnd As Integer = 0
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

            'Searching for end
            If SearchingForEnd >= 0 Then

                'Handle if statements
                If line.StartsWith(CONDITIONNAL_LINE_IF) OrElse line.StartsWith(CONDITIONNAL_LINE_IFNOT) Then
                    SearchingForEnd += 1
                ElseIf line.StartsWith(CONDITIONNAL_LINE_END) Then
                    SearchingForEnd -= 1
                End If

                'Goto next line
                Continue Do

            End If

            '$IF FLAG
            If line.StartsWith(CONDITIONNAL_LINE_IF) Then
                line = line.Substring(CONDITIONNAL_LINE_IF.Length)

                'If flag is not defined, goto end
                If Not Program.DefinedFlag.Contains(line) Then
                    SearchingForEnd = 0
                Else
                    ExceptEnd += 1
                End If

                'Goto next line
                Continue Do

            End If

            '$IFNOT FLAG
            If line.StartsWith(CONDITIONNAL_LINE_IFNOT) Then
                line = line.Substring(CONDITIONNAL_LINE_IFNOT.Length)

                'If flag is not defined, goto end
                If Program.DefinedFlag.Contains(line) Then
                    SearchingForEnd = 0
                Else
                    ExceptEnd += 1
                End If

                'Goto next line
                Continue Do

            End If

            '$END
            If line.StartsWith(CONDITIONNAL_LINE_END) Then
                ExceptEnd -= 1
                If ExceptEnd < 0 Then
                    Throw New ParsingLineUnexpectedElementException("No conditions are currently in place, therefore no $END was expected.", LinePosition, line, filepath)
                End If
                Continue Do
            End If

            'Empty line
            If line = "" Then
                Continue Do
            End If

            'Normal line content
            result.Add(New LimSourceLine(LinePosition, lineIndentation, line))

        Loop

        'Close file
        reader.Close()

        'Return result
        Return result

    End Function

    '
    ' Represent a line of code, have a tabulation
    '
    Public Class LimSourceLine

        ' Properties
        Public ReadOnly Tab As Integer
        Public ReadOnly Content As String
        Public ReadOnly LinePosition As Integer

        ' Constructor
        Public Sub New(LinePosition As Integer, Tab As Integer, Content As String)
            Me.LinePosition = LinePosition
            Me.Tab = Tab
            Me.Content = Content
        End Sub

        ' Tokenizer

    End Class

End Module
