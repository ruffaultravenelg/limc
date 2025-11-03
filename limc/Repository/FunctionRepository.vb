Namespace Repository
    Public Class FunctionRepository

        Private LazyFunctions As New List(Of Lazy.Function)

        Public Sub New(UncompiledFunctions As IEnumerable(Of AST.FunctionConstruct), CompilationContext As Context.Context)
            For Each UncompiledFunction In UncompiledFunctions
                LazyFunctions.Add(New Lazy.UserFunction(UncompiledFunction, CompilationContext))
            Next
        End Sub

        Public Function RetrieveFunction(Name As String) As Lazy.Function

            For Each Func In LazyFunctions
                If Func.Name = Name Then
                    Return Func
                End If
            Next

            Return Nothing
        End Function

    End Class
End Namespace