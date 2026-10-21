Imports System.Runtime.CompilerServices
Imports limc.Symbol

Namespace Repository.Comparer
    '
    ' Helps to handle hashmap keys : (Name, PassedGenerics) => (Name As String, PassedGenerics As IEnumerable(Of TypeSymbol))
    '
    Friend Class NameWithGenericComparer
        Implements IEqualityComparer(Of (Name As String, PassedGenerics As IEnumerable(Of TypeSymbol)))

        Public Overloads Function Equals(x As (Name As String, PassedGenerics As IEnumerable(Of TypeSymbol)), y As (Name As String, PassedGenerics As IEnumerable(Of TypeSymbol))) As Boolean Implements IEqualityComparer(Of (Name As String, PassedGenerics As IEnumerable(Of TypeSymbol))).Equals

            ' Element name
            If Not String.Equals(x.Name, y.Name) Then
                Return False
            End If

            ' If null collections
            If x.PassedGenerics Is Nothing AndAlso y.PassedGenerics Is Nothing Then Return True
            If x.PassedGenerics Is Nothing OrElse y.PassedGenerics Is Nothing Then Return False

            ' Check passed generic types equality
            Return x.PassedGenerics.SequenceEqual(y.PassedGenerics, ReferenceEqualityComparer.Instance)

        End Function

        Public Overloads Function GetHashCode(obj As (Name As String, PassedGenerics As IEnumerable(Of TypeSymbol))) As Integer Implements IEqualityComparer(Of (Name As String, PassedGenerics As IEnumerable(Of TypeSymbol))).GetHashCode
            Dim hash As New HashCode()

            ' Element name
            hash.Add(obj.Name)

            ' Add each generic type to hash
            If obj.PassedGenerics IsNot Nothing Then
                For Each T In obj.PassedGenerics
                    ' RuntimeHelpers.GetHashCode garantit qu'on prend le hash de l'instance mémoire, 
                    ' même si TypeSymbol surcharge la méthode GetHashCode()
                    hash.Add(RuntimeHelpers.GetHashCode(T))
                Next
            End If

            Return hash.ToHashCode()
        End Function
    End Class
End Namespace