Namespace Lim

    '
    ' A type represent a way to store data and interact with it.
    '
    Public MustInherit Class Type

        'Compiled name
        Public ReadOnly Property CompiledName As String

        'Name
        Public MustOverride ReadOnly Property Name As String

        'Generic types
        Public MustOverride ReadOnly Property GenericTypes As IEnumerable(Of Lim.Type)

        'TypeID
        Public ReadOnly Property TypeID As Integer
        Private Shared TypesIDs As Integer = 0

        'Constructor
        Public Sub New()

            'Create TypeID
            Lim.Type.TypesIDs += 1
            Me.TypeID = Lim.Type.TypesIDs

            'Create compiled name
            Me.CompiledName = C.Generator.Namer.GenerateTypeName()

        End Sub

        'Compile type
        Public MustOverride Sub Compile()

        'Equality
        Public Shared Operator =(a As Type, b As Type) As Boolean
            Return a.TypeID = b.TypeID
        End Operator
        Public Shared Operator <>(a As Type, b As Type) As Boolean
            Return Not a = b
        End Operator

        'To string
        Public Overrides Function ToString() As String
            If GenericTypes.Count = 0 Then
                Return Name
            Else
                Return Name & "<" & String.Join(", ", GenericTypes) & ">"
            End If
        End Function

    End Class

End Namespace