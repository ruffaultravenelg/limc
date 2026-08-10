Namespace Repository
    Public Class FunctionRepository

        ' Contains all functions with their C variants
        Private ReadOnly Functions As New Dictionary(Of String, FunctionSet)

        ' Repository constructor
        Public Sub New(UncompiledFunctions As IEnumerable(Of AST.FunctionConstruct), CompilationContext As Context.Context)
            For Each Fn In UncompiledFunctions
                If Not Functions.ContainsKey(Fn.Name) Then
                    Functions(Fn.Name) = New FunctionSet(CompilationContext)
                End If
                Functions(Fn.Name).RegisterModel(Fn)
            Next
        End Sub

        ' General repository endpoint
        Public Function RetrieveFunctions(Name As String, GenericTypes As IEnumerable(Of TypeSystem.Type)) As IEnumerable(Of Lazy.Function)
            If Functions.ContainsKey(Name) Then
                Return Functions(Name).RetrieveWithGenerics(GenericTypes)
            Else
                Return {}
            End If
        End Function

        ' Represent a set of functions with the same name
        Private Class FunctionSet

            Private Variants As New List(Of FunctionVariantContainer)
            Private CompilationContext As Context.Context

            Public Sub New(CompilationContext As Context.Context)
                Me.CompilationContext = CompilationContext
            End Sub

            Public Sub RegisterModel(Model As AST.FunctionConstruct)
                Variants.Add(New FunctionVariantContainer(Model, CompilationContext))
            End Sub

            Public Function RetrieveWithGenerics(GenericTypes As IEnumerable(Of TypeSystem.Type)) As IEnumerable(Of Lazy.Function)
                Dim Results As New List(Of Lazy.Function)

                For Each V In Variants
                    Dim RetrievedFunction As Lazy.Function = V.RetrieveFunction(GenericTypes)
                    If RetrievedFunction IsNot Nothing Then
                        Results.Add(RetrievedFunction)
                    End If
                Next

                Return Results
            End Function

        End Class

        ' Represent a function, contains all C subfunctions for each generic type combination
        Private Class FunctionVariantContainer

            Private Model As AST.FunctionConstruct
            Private CompilationContext As Context.Context
            Private LazyFunctions As New List(Of Lazy.Function)

            Public Sub New(Model As AST.FunctionConstruct, CompilationContext As Context.Context)
                Me.Model = Model
                Me.CompilationContext = CompilationContext
            End Sub

            Public Function RetrieveFunction(GenericTypes As IEnumerable(Of TypeSystem.Type)) As Lazy.Function

                ' Count error
                If GenericTypes.Count <> Model.GenericArguments.Count Then
                    Return Nothing
                End If

                ' Get the matching lazy function
                Dim MatchingLazyFunction As Lazy.Function = LazyFunctions.FirstOrDefault(Function(Fn) Fn.PassedGenericTypes.SequenceEqual(GenericTypes))

                ' If not found, create it
                If MatchingLazyFunction Is Nothing Then
                    Dim LazyFn As New Lazy.UserFunction(Model, GenericTypes, CompilationContext)
                    LazyFunctions.Add(LazyFn)
                    Return LazyFn
                Else
                    Return MatchingLazyFunction
                End If

            End Function

        End Class

    End Class
End Namespace