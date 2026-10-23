Imports limc.Symbol

Namespace HIR.Lowerer
    Friend Class CompilationQueue
        Private ReadOnly _queue As New Queue(Of ICompilableSymbol)()

        Public Sub Enqueue(symbol As ICompilableSymbol)
            If Not symbol.Compiled AndAlso Not _queue.Contains(symbol) Then
                _queue.Enqueue(symbol)
            End If
        End Sub

        Public Sub EnqueueRange(symbols As IEnumerable(Of ICompilableSymbol))
            For Each s In symbols
                Enqueue(s)
            Next
        End Sub

        Public Function HasWork() As Boolean
            Return _queue.Count > 0
        End Function

        Public Function Dequeue() As ICompilableSymbol
            Return _queue.Dequeue()
        End Function
    End Class
End Namespace