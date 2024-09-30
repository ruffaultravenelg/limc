Public Module GarbageCollector

    '=============================================
    '======== ALL GARBAGE COLLECTED TYPES ========
    '=============================================
    Private AllGarbageCollectedTypes As New List(Of IGarbageCollectedType)

    '======================================
    '======== ADD LINKED LIST HEAD ========
    '======================================
    Public Sub AddType(Type As IGarbageCollectedType)
        AllGarbageCollectedTypes.Add(Type)
    End Sub

    '======================================
    '======== GET LINKED LIST HEAD ========
    '======================================
    Public Iterator Function GetLinkedListHeads() As IEnumerable(Of String)
        For Each Type As IGarbageCollectedType In AllGarbageCollectedTypes
            Yield $"{Type.CompiledName} {Type.Name}_head = NULL;"
        Next
    End Function

    '==================================
    '======== GET MARK CONTENT ========
    '==================================
    Public Function GetMarkContent() As IEnumerable(Of String)

        'Create result
        Dim Result As New List(Of String) From {""}

        'Create mark for all heap types
        For Each Type As IGarbageCollectedType In AllGarbageCollectedTypes

            Dim Current As String = "current_" & Type.Name

            Result.Add($"{Type.CompiledName} {Current} = {Type.Name}_head;")
            Result.Add("while (" & Current & " != NULL) {")
            Result.Add(vbTab & "if (" & Current & "->stackReferences > 0) {")
            Result.Add(vbTab & vbTab & Type.Name & "_mark(" & Current & ");")
            Result.Add(vbTab & "}")
            Result.Add(vbTab & Current & " = " & Current & "->next;")
            Result.Add("}")
            Result.Add("")

        Next

        'Return result
        Return Result

    End Function

    '===================================
    '======== GET SWEEP CONTENT ========
    '===================================
    Public Function GetSweepContent() As IEnumerable(Of String)

        'Create result
        Dim Result As New List(Of String) From {""}

        'Debug
        If Program.GarbageCollectorDebug Then
            Result.Add($"unsigned long count;")
        End If

        'Create mark for all heap types
        For Each Type As IGarbageCollectedType In AllGarbageCollectedTypes

            Dim Current As String = "current_" & Type.Name
            Dim Previous As String = "previous_" & Type.Name

            Result.Add($"{Type.CompiledName} {Current} = {Type.Name}_head;")
            Result.Add($"{Type.CompiledName} {Previous} = NULL;")
            If Program.GarbageCollectorDebug Then
                Result.Add($"count = 0;")
            End If
            Result.Add("while (" & Current & " != NULL) {")
            Result.Add(vbTab & "if (!" & Current & "->marked) {")
            Result.Add(vbTab & vbTab & "if (" & Previous & " == NULL) {")
            Result.Add(vbTab & vbTab & vbTab & Type.Name & "_head = " & Current & "->next;")
            Result.Add(vbTab & vbTab & "} else {")
            Result.Add(vbTab & vbTab & vbTab & Previous & "->next = " & Current & "->next;")
            Result.Add(vbTab & vbTab & "}")
            If Program.GarbageCollectorDebug Then
                Result.Add(vbTab & vbTab & "count++;")
            End If
            Result.Add(vbTab & vbTab & Type.Name & "_free(" & Current & ");")
            Result.Add(vbTab & vbTab & Current & " = " & Previous & " ? " & Previous & "->next : " & Type.Name & "_head;")
            Result.Add(vbTab & "} else {")
            Result.Add(vbTab & vbTab & Previous & " = " & Current & ";")
            Result.Add(vbTab & vbTab & Current & " = " & Current & "->next;")
            Result.Add(vbTab & "}")
            Result.Add("}")
            If Program.GarbageCollectorDebug Then
                Result.Add($"printf(""%lu \""{Type.ToString()}\"" object%s %s freed.\n"", count, count > 1 ? ""s"":  """", count > 1 ? ""were"":  ""was"");")
            End If
            Result.Add("")

        Next

        'Return result
        Return Result

    End Function


    '=================================
    '======== GET RUN CONTENT ========
    '=================================
    Public Function GetRunContent() As IEnumerable(Of String)

        'Create result
        Dim Result As New List(Of String) From {
            "mark();",
            "sweep();"
        }

        'Return result
        Return Result

    End Function


End Module
