'
' Represent a line of code, have a tabulation
'
Public Class LimSourceLine

    ' Properties
    Public ReadOnly Tab As Integer 'Number of tabulation
    Public ReadOnly Content As String 'The content of the line (without tab)
    Public ReadOnly LinePosition As Integer 'Line number in the file (starts at 1)
    Public ReadOnly FirstContentCharPosition As Integer 'Where does the content of line really start (for 

    ' Constructor
    Public Sub New(LinePosition As Integer, FirstContentCharPosition As Integer, Tab As Integer, Content As String)
        Me.LinePosition = LinePosition
        Me.FirstContentCharPosition = FirstContentCharPosition
        Me.Tab = Tab
        Me.Content = Content
    End Sub

End Class