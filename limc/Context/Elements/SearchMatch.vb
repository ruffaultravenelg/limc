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
        MATCH_METHOD
        MATCH_CONSTANT
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

    'Method
    Public Sub New(Element As Lazy.Method)
        Me.New(Element, MatchType.MATCH_METHOD)
    End Sub
    Public ReadOnly Property MatchingMethod As Lazy.Method
        Get
            Return Result
        End Get
    End Property

    'Constant
    Public Sub New(Element As ConstantData)
        Me.New(Element, MatchType.MATCH_CONSTANT)
    End Sub
    Public ReadOnly Property MatchingConstant As ConstantData
        Get
            Return Result
        End Get
    End Property

End Class
