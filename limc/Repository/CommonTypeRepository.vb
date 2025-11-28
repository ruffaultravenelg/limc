Namespace Repository
    Public Class CommonTypeRepository

        Private TypeConstructs As IEnumerable(Of IGenerateType)
        Private CompiledTypes As New List(Of TypeSystem.CommonType)

        Public Sub New(TypeConstructs As IEnumerable(Of IGenerateType))
            Me.TypeConstructs = TypeConstructs
        End Sub

        Public Function RetrieveType(Name As String, GenericTypes As IEnumerable(Of TypeSystem.Type), OnlyExported As Boolean) As TypeSystem.Type

            ' Search in compiled types
            For Each T In CompiledTypes
                If OnlyExported AndAlso Not T.ConstructIsExported Then
                    Continue For
                End If
                If T.Name = Name AndAlso T.GenericTypes.SequenceEqual(GenericTypes) Then
                    Return T
                End If
            Next

            ' Search in not compiled types
            For Each T In TypeConstructs
                If OnlyExported AndAlso Not DirectCast(T, AST.ConstructNode).Exported Then
                    Continue For
                End If
                If T.DoMatchTypeInfo(Name, GenericTypes) Then
                    Dim NewType As TypeSystem.CommonType = T.InstanciateType(GenericTypes)
                    CompiledTypes.Add(NewType)
                    NewType.Compile()
                    Return NewType
                End If
            Next

            ' Not found
            Return Nothing

        End Function

    End Class
End Namespace