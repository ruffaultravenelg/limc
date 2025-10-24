Public Class SearchMatch

    Public ReadOnly Property Type As MatchType
    Private ReadOnly Result As Object

    Public Sub New(Result As Object, Type As MatchType)
        Me.Type = Type
        Me.Result = Result
    End Sub

    Public Enum MatchType
        MATCH_FUNCTION
        MATCH_VARIABLE
    End Enum

    Public ReadOnly Property MatchingFunction As Lazy.Function
        Get
            Return Result
        End Get
    End Property
    Public ReadOnly Property MatchingVariable As Scope.VariableData
        Get
            Return Result
        End Get
    End Property

End Class
