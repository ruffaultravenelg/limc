Public MustInherit Class FunctionContainer

    'Functions
    Dim CompiledFunctions As New Dictionary(Of String, HashSet(Of Lim.Function))
    Dim UncompiledFunctions As New Dictionary(Of String, HashSet(Of Source.Function))

    'Constructor
    Public Sub New(Functions As IEnumerable(Of Source.Function))
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

    'Compile function
    Private Sub CompileFunction(Fn As Source.Function, GenericTypes As IEnumerable(Of Lim.Type))

        'Create object
        Dim CompiledFunction As New Lim.Function(Fn, GenericTypes)

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
        For Each Fn As Source.Function In UncompiledFunctions(Name)
            Dim CompiledFunction As New Lim.Function(Fn, GenericTypes)

        Next

        'Get all compiled functions
        Correspondances.AddRange(Correspondances(Name))

        'Return value
        Return Correspondances

    End Function

End Class
