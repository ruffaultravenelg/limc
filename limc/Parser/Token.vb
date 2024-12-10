Public Class Token

    ' Properties
    Public ReadOnly Property Type As TokenType
    Public ReadOnly Property Value As String
    Public ReadOnly Property Location As Location

    ' Constructor
    Public Sub New(Type As TokenType, Location As Location)
        Me.Type = Type
        Me.Value = Nothing
        Me.Location = Location
    End Sub
    Public Sub New(Type As TokenType, Value As String, Location As Location)
        Me.Type = Type
        Me.Value = Value
        Me.Location = Location
    End Sub

    'To string
    Public Overrides Function ToString() As String
        If Value IsNot Nothing Then
            Return "[" & Type.ToString() & ", " & Value.ToString() & "]"
        Else
            Return "[" & Type.ToString() & "]"
        End If
    End Function

    ' Token types
    Public Enum TokenType
        WORD

        VALUE_INT
        VALUE_FLOAT
        VALUE_STRING
        VALUE_FSTRING
        VALUE_TRUE
        VALUE_FALSE

        KEYWORD_FUNC
        KEYWORD_RETURN
    End Enum

End Class
