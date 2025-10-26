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


    'Variable
    Public Sub New(Element As VariableData)
        Me.New(Element, MatchType.MATCH_VARIABLE)
    End Sub
    Public ReadOnly Property MatchingVariable As VariableData
        Get
            Return Result
        End Get
    End Property


    'Function
    Public Sub New(Element As Lazy.Function)
        Me.New(Element, MatchType.MATCH_FUNCTION)
    End Sub

    Public ReadOnly Property MatchingFunction As Lazy.Function
        Get
            Return Result
        End Get
    End Property

End Class
