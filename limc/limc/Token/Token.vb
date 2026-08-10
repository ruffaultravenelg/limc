Public Class Token

    Public ReadOnly Property Type As TokenType
    Public ReadOnly Property Value As Object
    Public ReadOnly Property Location As Location

    Public Sub New(Type As TokenType, Location As Location, Optional Value As Object = Nothing)
        Me.Type = Type
        Me.Location = Location
        Me.Value = Value
    End Sub

    Public Overrides Function ToString() As String
        If Value Is Nothing Then
            Return $"[{Type.ToString()}]"
        Else
            Return $"[{Type.ToString()}, ""{Value.ToString()}""]"
        End If
    End Function

End Class
