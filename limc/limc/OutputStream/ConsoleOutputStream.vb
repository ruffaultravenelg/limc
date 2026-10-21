Namespace OutputStream
    Public Class ConsoleOutputStream
        Implements IOutputStream

        Public Sub Writeline(Optional Value As String = "") Implements IOutputStream.Writeline
            Console.WriteLine(Value)
        End Sub

        Public Sub Write(Value As String) Implements IOutputStream.Write
            Console.Write(Value)
        End Sub

        Public Sub SetColor(Value As IOutputStream.Color) Implements IOutputStream.SetColor
            Console.ForegroundColor = ResolveColor(Value)
        End Sub

        Public Sub ResetColor() Implements IOutputStream.ResetColor
            Console.ResetColor()
        End Sub

        Private Shared Function ResolveColor(Color As IOutputStream.Color) As ConsoleColor
            Select Case Color
                Case IOutputStream.Color.Grey
                    Return ConsoleColor.Gray

                Case IOutputStream.Color.White
                    Return ConsoleColor.White

                Case IOutputStream.Color.Red
                    Return ConsoleColor.Red

                Case IOutputStream.Color.Green
                    Return ConsoleColor.Green

                Case Else
                    Throw New InternalException("Unknown color to convert to ConsoleColor")
            End Select
        End Function

    End Class
End Namespace