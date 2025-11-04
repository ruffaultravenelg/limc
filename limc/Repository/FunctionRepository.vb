Namespace Repository
    Public Class FunctionRepository

        Private LazyFunctions As New List(Of Lazy.Function)
        Private UncompiledFunctions As IEnumerable(Of AST.FunctionConstruct)
        Private CompilationContext As Context.Context
        Public Sub New(UncompiledFunctions As IEnumerable(Of AST.FunctionConstruct), CompilationContext As Context.Context)
            Me.UncompiledFunctions = UncompiledFunctions
            Me.CompilationContext = CompilationContext
        End Sub

        Public Function RetrieveFunction(Name As String, GenericTypes As IEnumerable(Of TypeSystem.Type)) As Lazy.Function

            ' Search from known lazy functions
            Dim LazyFunction As Lazy.Function = SearchFromLazy(Name, GenericTypes)
            If LazyFunction IsNot Nothing Then
                Return LazyFunction
            End If

            ' Search from uncompiled functions
            LazyFunction = SearchFromUncompiled(Name, GenericTypes)
            If LazyFunction IsNot Nothing Then
                Return LazyFunction
            End If

            ' Nothing found
            Return Nothing

        End Function

        Public Function SearchFromLazy(Name As String, GenericTypes As IEnumerable(Of TypeSystem.Type)) As Lazy.Function
            For Each Func In LazyFunctions
                If Func.DoMatch(Name, GenericTypes) Then
                    Return Func
                End If
            Next
            Return Nothing
        End Function

        Public Function SearchFromUncompiled(Name As String, GenericTypes As IEnumerable(Of TypeSystem.Type)) As Lazy.Function
            For Each UncompiledFunction In UncompiledFunctions
                If UncompiledFunction.DoMatch(Name, GenericTypes) Then
                    Dim Fn As New Lazy.UserFunction(UncompiledFunction, GenericTypes, CompilationContext)
                    LazyFunctions.Add(Fn)
                    Return Fn
                End If
            Next
            Return Nothing
        End Function

    End Class
End Namespace