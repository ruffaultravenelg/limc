Namespace TypeSystem
    Public MustInherit Class CommonType
        Inherits Type

        ' Common type identifications
        Public ReadOnly Property Name As String
        Public ReadOnly Property GenericTypes As IEnumerable(Of TypeSystem.Type)
        Public ReadOnly Property ConstructIsExported As Boolean

        ' All recursion (like typeNode.AssociatedType) must be done in Compile()
        Public Overridable Sub Compile()

        End Sub

        ' Construcor
        Public Sub New(Name As String, GenericTypes As IEnumerable(Of TypeSystem.Type), ConstructIsExported As Boolean)
            Me.Name = Name
            Me.GenericTypes = GenericTypes
            Me.ConstructIsExported = ConstructIsExported
        End Sub

        ' To string
        Public Overrides Function ToString() As String
            Return Name & "<" & String.Join(", ", GenericTypes) & ">"
        End Function

    End Class
End Namespace