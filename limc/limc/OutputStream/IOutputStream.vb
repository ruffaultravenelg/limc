Namespace OutputStream
    Public Interface IOutputStream
        Sub Writeline(Optional Value As String = "")
        Sub Write(Value As String)
        Sub SetColor(Value As Color)
        Sub ResetColor()

        Enum Color
            Grey
            White
            Red
            Green
        End Enum

    End Interface
End Namespace