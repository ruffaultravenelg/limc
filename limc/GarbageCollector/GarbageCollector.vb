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


End Module
