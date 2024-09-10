Public Class AnnotationNode
    Inherits Node

    ' Properties
    Public ReadOnly Name As String
    Public ReadOnly Args As New List(Of String)

    ' Constructor
    Public Sub New(Location As Location, Word As String)

        ' Init super
        MyBase.New(Location)

        ' Parse Word into Name and Args
        Dim startParenIndex As Integer = Word.IndexOf("("c)

        If startParenIndex = -1 Then
            ' No arguments, the word is just the name
            Name = Word
        Else
            ' Extract the name (everything before the '(')
            Name = Word.Substring(0, startParenIndex).Trim()

            ' Extract the arguments (everything between '(' and ')')
            Dim endParenIndex As Integer = Word.IndexOf(")"c, startParenIndex)
            If endParenIndex > startParenIndex Then
                Dim argsString As String = Word.Substring(startParenIndex + 1, endParenIndex - startParenIndex - 1)

                ' Split arguments by comma, and trim whitespace
                Args.AddRange(argsString.Split(","c).Select(Function(arg) arg.Trim()))
            End If
        End If

    End Sub

End Class
