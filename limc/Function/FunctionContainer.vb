Public Class FunctionContainer

    'Functions
    Private CompiledFunctions As New Dictionary(Of String, HashSet(Of Lim.Function))
    Private UncompiledFunctions As New Dictionary(Of String, HashSet(Of Source.Function))
    Private ContextForFunctionCompilation As Context

    'Constructor
    Public Sub New(Functions As IEnumerable(Of Source.Function), Optional ContextForFunctionCompilation As Context = Nothing)

        Me.ContextForFunctionCompilation = ContextForFunctionCompilation

        For Each Fn As Source.Function In Functions
            AddFunction(Fn)
        Next

    End Sub

    'Add a function
    Private Sub AddFunction(Fn As Source.Function)

        'Create hashset if not exist
        If Not UncompiledFunctions.ContainsKey(Fn.Name) Then
            UncompiledFunctions.Add(Fn.Name, New HashSet(Of Source.Function))
        End If

        'Add to hashset
        UncompiledFunctions(Fn.Name).Add(Fn)

    End Sub

    'Same functions ? according to generic types only
    Private Function TypeListAreTheSame(A As IEnumerable(Of Lim.Type), B As IEnumerable(Of Lim.Type)) As Boolean

        'Count
        If A.Count <> B.Count Then
            Return False
        End If

        'Compare each elements
        For i As Integer = 0 To A.Count - 1
            If A(i) <> B(i) Then
                Return False
            End If
        Next

        'Everything is OK
        Return True

    End Function

    'Function match arguments
    Private Function FunctionMatchArguments(Fn As Lim.Function, Arguments As IEnumerable(Of Lim.Type)) As Boolean

        'Argument count
        If Not Fn.Arguments.Count = Arguments.Count Then
            Return False
        End If

        'Argument matches
        For i As Integer = 0 To Arguments.Count - 1
            If Not Fn.Arguments(i) = Arguments(i) Then
                Return False
            End If
        Next

        'All tests pass
        Return True

    End Function

    'Compile function
    Private Sub CompileFunction(Fn As Source.Function, GenericTypes As IEnumerable(Of Lim.Type))

        'Search if the function was already compiled
        If CompiledFunctions.ContainsKey(Fn.Name) Then
            For Each AlreadyCompiledFunction As Lim.Function In CompiledFunctions(Fn.Name)
                If TypeListAreTheSame(AlreadyCompiledFunction.GenericTypes, GenericTypes) Then
                    Return 'A compiled functions have the same name / same generic types => don't compile the source function
                End If
            Next
        End If

        'Create object
        Dim CompiledFunction As New Lim.Function(Fn, GenericTypes, ContextForFunctionCompilation)

        'Create hashset if not exist
        If Not CompiledFunctions.ContainsKey(Fn.Name) Then
            CompiledFunctions.Add(Fn.Name, New HashSet(Of Lim.Function))
        End If

        'Add object into hashset
        CompiledFunctions(Fn.Name).Add(CompiledFunction)

        'Compile the function
        CompiledFunction.Compile()

    End Sub

    'Get correspondances -> {Name, GenericType}
    Public Function GetCorrespondances(Name As String, GenericTypes As IEnumerable(Of Lim.Type)) As List(Of Lim.Function)

        'Create result (list because it's ordered from the best anwser to the worst, for other correspondance functions)
        Dim Correspondances As New List(Of Lim.Function)

        'Compile uncompiled functions that match
        If UncompiledFunctions.ContainsKey(Name) Then
            For Each Fn As Source.Function In UncompiledFunctions(Name)
                If Fn.GenericTypes.Count = GenericTypes.Count Then
                    CompileFunction(Fn, GenericTypes)
                End If
            Next
        End If

        'Get all compiled functions
        If CompiledFunctions.ContainsKey(Name) Then
            Correspondances.AddRange(CompiledFunctions(Name))
        End If

        'Return value
        Return Correspondances

    End Function

    'Get correspondances ->  {Name, GenericType, Arguments}
    Public Function GetCorrespondance(Name As String, GenericTypes As IEnumerable(Of Lim.Type), Arguments As IEnumerable(Of Lim.Type)) As Lim.Function

        'Get correspondances of name & generic types
        Dim Correspondances As New List(Of Lim.Function)

        'Search the one that correspond
        For Each NameCorrespondance As Lim.Function In GetCorrespondances(Name, GenericTypes)
            If FunctionMatchArguments(NameCorrespondance, Arguments) Then
                Correspondances.Add(NameCorrespondance)
            End If
        Next

        'On correspondance -> ok
        If Correspondances.Count = 1 Then
            Return Correspondances.First
        End If

        'Multiple correspondance -> error
        If Correspondances.Count > 1 Then
            Throw New SyntaxException("To functions have the same signatures", Correspondances.First.Base.Location)
        End If

        'No correspondance
        Return Nothing

    End Function

End Class
