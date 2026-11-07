Imports System.Runtime.CompilerServices
Imports limc.Symbol

Namespace Repository.Comparer
    ' === COMPARER (for func types hashmap) ====
    Friend Class FuncTypeComparer
        Implements IEqualityComparer(Of (Arguments As IEnumerable(Of TypeSymbol), Ret As TypeSymbol))

        Public Overloads Function Equals(x As (Arguments As IEnumerable(Of TypeSymbol), Ret As TypeSymbol), y As (Arguments As IEnumerable(Of TypeSymbol), Ret As TypeSymbol)) As Boolean Implements IEqualityComparer(Of (Arguments As IEnumerable(Of TypeSymbol), Ret As TypeSymbol)).Equals

            ' Element name
            If x.Ret IsNot Nothing AndAlso y.Ret IsNot Nothing Then 'there 2 types => compare them
                If Not x.Ret.Equals(y.Ret) Then
                    Return False
                End If
            ElseIf x.Ret IsNot Nothing OrElse y.Ret IsNot Nothing Then 'theres 1 type and other is nothing
                Return False
            End If

            ' If null collections
            If x.Arguments Is Nothing AndAlso y.Arguments Is Nothing Then Return True
            If x.Arguments Is Nothing OrElse y.Arguments Is Nothing Then Return False

            ' Check passed generic types equality
            Return x.Arguments.SequenceEqual(y.Arguments, ReferenceEqualityComparer.Instance)

        End Function

        Public Overloads Function GetHashCode(obj As (Arguments As IEnumerable(Of TypeSymbol), Ret As TypeSymbol)) As Integer Implements IEqualityComparer(Of (Arguments As IEnumerable(Of TypeSymbol), Ret As TypeSymbol)).GetHashCode
            Dim hash As New HashCode()

            hash.Add(obj.Ret)

            If obj.Arguments IsNot Nothing Then
                For Each T In obj.Arguments
                    ' RuntimeHelpers.GetHashCode garantit qu'on prend le hash de l'instance mémoire, 
                    ' même si TypeSymbol surcharge la méthode GetHashCode()
                    hash.Add(RuntimeHelpers.GetHashCode(T))
                Next
            End If

            Return hash.ToHashCode()
        End Function
    End Class
End Namespace