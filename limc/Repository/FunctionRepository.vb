Namespace Repository
    Public Class FunctionRepository

        ' Contains all functions with their C variants
        Private ReadOnly Functions As New Dictionary(Of String, FunctionChilds)

        ' Repository constructor
        Public Sub New(UncompiledFunctions As IEnumerable(Of AST.FunctionConstruct), CompilationContext As Context.Context)
            For Each Fn In UncompiledFunctions
                If Functions.ContainsKey(Fn.Name) Then
                    Throw New InternalError()
                End If
                Functions(Fn.Name) = New FunctionChilds(Fn, CompilationContext)
            Next
        End Sub

        ' General repository endpoint
        Public Function RetrieveFunction(Name As String, GenericTypes As IEnumerable(Of TypeSystem.Type)) As Lazy.Function
            If Functions.ContainsKey(Name) Then
                Return Functions(Name).RetrieveWithGenerics(GenericTypes)
            Else
                Return Nothing
            End If
        End Function

        ' Represent a specifi function, contains all C subfunctions
        Private Class FunctionChilds

            Public ReadOnly Property Model As AST.FunctionConstruct
            Private CompilationContext As Context.Context

            Private LazyFunctions As New List(Of Lazy.Function)

            Public Sub New(Model As AST.FunctionConstruct, CompilationContext As Context.Context)
                Me.Model = Model
                Me.CompilationContext = CompilationContext
            End Sub

            Public Function RetrieveWithGenerics(GenericTypes As IEnumerable(Of TypeSystem.Type)) As Lazy.Function

                ' Count error
                If GenericTypes.Count <> Model.GenericArguments.Count Then
                    Throw New InternalError()
                End If

                ' Search if there is a function already defined
                For Each Fn In LazyFunctions

                    ' Check passed generic types match
                    Dim AllGood As Boolean = True
                    For I As Integer = 0 To GenericTypes.Count - 1
                        If Fn.PassedGenericTypes(I) <> GenericTypes(I) Then
                            AllGood = False
                            Exit For
                        End If
                    Next

                    ' If ok return this function
                    If AllGood Then
                        Return Fn
                    End If

                Next

                ' No function returned -> compile a new one
                Dim NewFn As New Lazy.UserFunction(Model, GenericTypes, CompilationContext)
                LazyFunctions.Add(NewFn)
                Return NewFn

            End Function

        End Class

    End Class
End Namespace