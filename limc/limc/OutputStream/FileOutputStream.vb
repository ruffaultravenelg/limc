Namespace OutputStream
    Public Class FileOutputStream
        Implements IOutputStream
        Implements IDisposable

        Private _writer As IO.StreamWriter
        Private _disposedValue As Boolean

        Public Sub New(OutputFilepath As String)
            _writer = New IO.StreamWriter(OutputFilepath)
        End Sub


        Public Sub Writeline(Optional Value As String = "") Implements IOutputStream.Writeline
            _writer.WriteLine(Value)
        End Sub

        Public Sub Write(Value As String) Implements IOutputStream.Write
            _writer.Write(Value)
        End Sub

        Public Sub SetColor(Value As IOutputStream.Color) Implements IOutputStream.SetColor
            ' Do nothing
        End Sub
        Public Sub ResetColor() Implements IOutputStream.ResetColor
            ' Do nothing
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _disposedValue Then
                If disposing Then
                    If _writer IsNot Nothing Then
                        _writer.Dispose()
                        _writer = Nothing
                    End If
                End If
                _disposedValue = True
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub

    End Class
End Namespace