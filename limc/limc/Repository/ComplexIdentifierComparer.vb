Namespace Repository
    Friend Class ComplexIdentifierComparer
        Implements IEqualityComparer(Of (Name As String, PassedGenerics As IEnumerable(Of Symbol.TypeSymbol)))

        Public Function Equals1(
            x As (Name As String, PassedGenerics As IEnumerable(Of Symbol.TypeSymbol)),
            y As (Name As String, PassedGenerics As IEnumerable(Of Symbol.TypeSymbol))
        ) As Boolean Implements IEqualityComparer(Of (Name As String, PassedGenerics As IEnumerable(Of Symbol.TypeSymbol))).Equals

            ' Compare names
            If x.Name <> y.Name Then Return False

            ' Compare passed generics
            If x.PassedGenerics Is Nothing AndAlso y.PassedGenerics Is Nothing Then Return True ' 2 ares null
            If x.PassedGenerics Is Nothing OrElse y.PassedGenerics Is Nothing Then Return False ' 1 only is null

            Return x.PassedGenerics.SequenceEqual(y.PassedGenerics)
        End Function

        Public Function GetHashCode1(obj As (Name As String, PassedGenerics As IEnumerable(Of Symbol.TypeSymbol))) As Integer Implements IEqualityComparer(Of (Name As String, PassedGenerics As IEnumerable(Of Symbol.TypeSymbol))).GetHashCode
            Dim hc As New HashCode()
            hc.Add(obj.Name)

            If obj.PassedGenerics IsNot Nothing Then
                For Each item In obj.PassedGenerics
                    hc.Add(item)
                Next
            End If

            Return hc.ToHashCode()
        End Function
    End Class
End Namespace