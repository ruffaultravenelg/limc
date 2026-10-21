Namespace C.Naming
    Friend Class ManglerNamer
        Implements ISymbolNamer

        Private Map As New Dictionary(Of Symbol.Symbol, String)
        Private AtomicCounter As Integer = 0

        ' Constuctor
        Public Sub New()
            PopulateKnownCompiledNames()
        End Sub

        ' Define knowns type (such as primitive int, float, etc)
        Private Sub PopulateKnownCompiledNames()
            Map(Symbol.TypeSymbol.INT) = "int"
        End Sub

        ' Create a custom name from a symbol
        Private Function GenerateName(s As Symbol.Symbol) As String
            AtomicCounter += 1
            Return $"lims_{Sanitize(s.LimName)}_{AtomicCounter}"
        End Function

        ' Sanitize lim name into c name
        Private Shared Function Sanitize(input As String) As String
            Dim sb As New Text.StringBuilder()

            For Each ch In input
                Select Case ch
                    Case "a"c To "z"c, "A"c To "Z"c, "0"c To "9"c, "_"c
                        sb.Append(ch)
                    Case Else
                        sb.Append($"_x{AscW(ch):X2}_")
                End Select
            Next

            Return sb.ToString()
        End Function

        ' Get a compiledname for a given symbol (known or not)
        Public Function GetCompiledName(Symbol As Symbol.Symbol) As String Implements ISymbolNamer.GetCompiledName
            If Map.ContainsKey(Symbol) Then
                Return Map(Symbol)
            Else
                Dim CompiledName As String = GenerateName(Symbol)
                Map(Symbol) = CompiledName
                Return CompiledName
            End If
        End Function

    End Class
End Namespace