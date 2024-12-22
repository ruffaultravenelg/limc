Namespace Lim

    '
    ' A type represent a way to store data and interact with it.
    '
    Public MustInherit Class Type

        'TypeID
        Public ReadOnly Property TypeID As Integer
        Private Shared TypesIDs As Integer = 0

        'Constructor
        Public Sub New()

            'Create TypeID
            Lim.Type.TypesIDs += 1
            Me.TypeID = Lim.Type.TypesIDs

        End Sub

        'Equality
        Public Shared Operator =(a As Type, b As Type) As Boolean
            Return a.TypeID = b.TypeID
        End Operator
        Public Shared Operator <>(a As Type, b As Type) As Boolean
            Return Not a = b
        End Operator

    End Class

End Namespace