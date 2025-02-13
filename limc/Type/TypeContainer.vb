Public Class TypeContainer

    'Uncompiled types
    Private UncompiledTypes As IEnumerable(Of TypeConstruct)
    Private CompiledTypes As New List(Of Lim.Type)

    'Constructor
    Public Sub New(Types As IEnumerable(Of TypeConstruct))
        Me.UncompiledTypes = Types
    End Sub

    'Compiled class correspond
    Private Function CompiledTypeCorrespond(Type As Lim.Type, Name As String, GenericTypes As IEnumerable(Of Lim.Type)) As Boolean

        'Compare name
        If Not Type.Name = Name Then
            Return False
        End If

        'Argument count
        If Not Type.PassedGenericTypes.Count = GenericTypes.Count Then
            Return False
        End If

        'Compares generic types
        For i As Integer = 0 To GenericTypes.Count - 1
            If Not Type.PassedGenericTypes(i) = GenericTypes(i) Then
                Return False
            End If
        Next

        'Every tests passed
        Return True

    End Function

    'Uncompiled class correspond
    Private Function UncompiledTypeCorrespond(Type As TypeConstruct, Name As String, GenericTypes As IEnumerable(Of Lim.Type)) As Boolean

        'Compare name
        If Not Type.Name = Name Then
            Return False
        End If

        'Argument count
        If Not Type.GenericTypes.Count = GenericTypes.Count Then
            Return False
        End If

        'Every tests passed
        Return True

    End Function


    'Get correspondance
    Public Function GetCorrespondance(Name As String, GenericTypes As IEnumerable(Of Lim.Type)) As Lim.Type

        'Search for compiled type
        For Each Type As Lim.Type In CompiledTypes
            If CompiledTypeCorrespond(Type, Name, GenericTypes) Then
                Return Type
            End If
        Next

        'Search for uncompiled type
        For Each Type As TypeConstruct In UncompiledTypes
            If UncompiledTypeCorrespond(Type, Name, GenericTypes) Then

                'Create lim type
                Dim LimType As Lim.Type = Type.Compile(GenericTypes)

                'Add it to compiled types
                CompiledTypes.Add(LimType)

                'Compile the lim type
                LimType.Compile()

                'Return the type
                Return LimType

            End If
        Next

        'Nothing found
        Return Nothing

    End Function

End Class
