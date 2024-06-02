Imports System.IO

Public Class Token

    Public ReadOnly Type As TokenType
    Public ReadOnly Value As String
    Public ReadOnly Location As Location

    Public Sub New(Location As Location, Type As TokenType, Optional Value As String = "")
        Me.Type = Type
        Me.Value = Value
        Me.Location = Location
    End Sub

    Public Overrides Function ToString() As String
        If Value = "" Then
            Return $"[{Type.ToString()}]"
        Else
            Return $"[{Type.ToString()}, {Value.ToString()}]"
        End If
    End Function

    Public Shared Function StringifyListOfToken(Tokens As List(Of Token)) As String

        'Empty list
        If Tokens.Count = 0 Then
            Return ""
        End If

        'Convert all tokens
        Dim Result As String = ""
        For Each Token As Token In Tokens
            Result &= " " & Token.ToString()
        Next
        Return Result.Substring(1)

    End Function

    Public Sub Serialize(writer As BinaryWriter)
        writer.Write(Type.ToString())
        writer.Write(Value)
    End Sub

    Public Shared Function Deserialize(reader As BinaryReader, Location As Location) As Token

        ' Read type
        Dim typeString As String = reader.ReadString()
        Dim type As TokenType = [Enum].Parse(GetType(TokenType), typeString)

        ' Read value
        Dim value As String = reader.ReadString()

        ' Location is not serialized
        Return New Token(Location, type, value)

    End Function

End Class