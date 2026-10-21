Namespace Lexer

    Friend Class Token
        Public ReadOnly Property Type As TokenType
        Public ReadOnly Property Value As Object
        Public ReadOnly Property Location As Location

        Public Sub New(Type As TokenType, Value As Object, Location As Location)
            Me.Type = Type
            Me.Value = Value
            Me.Location = Location
        End Sub

        Public Overrides Function ToString() As String
            If String.IsNullOrEmpty(Value) Then
                Return $"[{Type}]"
            Else
                Return $"[{Type}, {Value}]"
            End If
        End Function

        Public Function ToEnglish() As String
            Return TokenTypeToEnglish(Type)
        End Function

    End Class

End Namespace