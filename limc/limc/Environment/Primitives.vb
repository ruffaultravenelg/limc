Imports System.Runtime.CompilerServices
Imports limc.Symbol

Namespace Environment
    Friend Class Primitives

        ' ==== MAIN TYPES ====
        Public Shared ReadOnly INT As New IntTypeSymbol()
        Public Shared ReadOnly FLOAT As New FloatTypeSymbol()
        Public Shared ReadOnly BOOL As New BoolTypeSymbol()
        Public Shared ReadOnly STR As New StrTypeSymbol()

        ' func<a, b><r>
        Private Shared ReadOnly FuncTypes As New Dictionary(Of (Arguments As IEnumerable(Of TypeSymbol), Ret As TypeSymbol), FuncTypeSymbol)(New FuncTypeComparer)
        Public Shared Function Func(ArgumentTypes As IEnumerable(Of TypeSymbol), ReturnType As TypeSymbol) As FuncTypeSymbol
            Dim Key = (ArgumentTypes, ReturnType)

            If FuncTypes.ContainsKey(Key) Then
                Return FuncTypes(Key)
            Else
                Dim newSymbol As New FuncTypeSymbol(ArgumentTypes, ReturnType)
                FuncTypes(Key) = newSymbol
                Return newSymbol
            End If

        End Function


        ' === COMPARER (for func types hashmap) ====
        Private Class FuncTypeComparer
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


    End Class
End Namespace