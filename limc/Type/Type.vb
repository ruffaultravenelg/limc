Namespace Lim

    '
    ' A type represent a way to store data and interact with it.
    '
    Public MustInherit Class Type

        'Compiled name
        Private _CompiledName As String
        Public ReadOnly Property CompiledName As String
            Get
                Return _CompiledName
            End Get
        End Property
        Protected Sub SetCompiledName(Value As String)
            _CompiledName = Value
        End Sub

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
            SetCompiledName(C.Generator.Namer.GenerateTypeName())

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

        'Default value
        Public MustOverride Function DefaultValue() As String

        'Assignation
        Public MustOverride Function Assignation(Variable As String, Value As String) As String

    End Class

End Namespace